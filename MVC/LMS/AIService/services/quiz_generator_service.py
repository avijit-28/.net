"""
Quiz Generator Service for RAG AI Learning Assistant.
Generates structured practice quizzes (MCQs, True/False, Short Answer)
grounded in retrieved course materials or lesson contents.
"""
import json
import re
from typing import List, Dict, Any, Optional
from pydantic import BaseModel, Field
from llm.provider_factory import LLMProviderFactory
from rag.prompt_templates import SystemPrompts
from core.logger import get_masked_logger

logger = get_masked_logger(__name__)

class QuizQuestion(BaseModel):
    id: int
    question: str
    type: str = "mcq"  # "mcq", "true_false", "short_answer"
    options: List[str] = Field(default_factory=list)
    correct_answer: str
    explanation: str
    difficulty: str = "Intermediate"
    source_reference: Optional[str] = None

class QuizResult(BaseModel):
    course_id: int
    title: str
    total_questions: int
    difficulty: str
    questions: List[QuizQuestion]
    grounded_in_rag: bool = True

class QuizGeneratorService:
    def __init__(self):
        self.llm = LLMProviderFactory.get_provider()

    async def generate_quiz(
        self,
        course_id: int,
        course_title: str,
        context_chunks: List[str],
        num_questions: int = 5,
        difficulty: str = "Intermediate",
        question_type: str = "mcq",
        topic: Optional[str] = None
    ) -> QuizResult:
        """
        Generates practice questions grounded in context chunks.
        Never publishes directly to official graded exams.
        """
        logger.info(f"Generating practice quiz for course {course_id} ({num_questions} questions, {difficulty})")
        
        context_text = "\n\n---\n\n".join(context_chunks)
        if not context_text.strip():
            context_text = f"Course: {course_title}. General curriculum concepts for {topic or 'the course'}."

        prompt = f"""
You are an expert educational assessment creator for {course_title}.
Generate a {difficulty}-level practice quiz of {num_questions} questions on: {topic or 'the provided course materials'}.
Question type: {question_type}.

GROUNDING CONTEXT:
{context_text}

INSTRUCTIONS:
1. Every question must be directly grounded in the provided context.
2. Return ONLY valid JSON in the exact format shown below, with no markdown code fences or conversational text.
3. For MCQ: provide exactly 4 distinct options (A, B, C, D) and specify the exact correct option string.
4. Provide a clear pedagogical explanation for the correct answer.

JSON FORMAT:
{{
  "title": "{course_title} Practice Quiz",
  "difficulty": "{difficulty}",
  "questions": [
    {{
      "id": 1,
      "question": "Question text here?",
      "type": "{question_type}",
      "options": ["Option A", "Option B", "Option C", "Option D"],
      "correct_answer": "Option A",
      "explanation": "Explanation why this is correct based on the text.",
      "difficulty": "{difficulty}",
      "source_reference": "Section or concept reference"
    }}
  ]
}}
"""
        response_text, tokens, provider = await self.llm.generate(
            prompt=prompt,
            system_prompt=SystemPrompts.QUIZ_GENERATOR,
            temperature=0.3
        )

        try:
            # Clean JSON if wrapped in markdown code fence
            clean_json = re.sub(r"^```(?:json)?\s*", "", response_text.strip(), flags=re.MULTILINE)
            clean_json = re.sub(r"```$", "", clean_json.strip(), flags=re.MULTILINE).strip()
            data = json.loads(clean_json)
            
            questions = [QuizQuestion(**q) for q in data.get("questions", [])]
            return QuizResult(
                course_id=course_id,
                title=data.get("title", f"{course_title} Practice Quiz"),
                total_questions=len(questions),
                difficulty=difficulty,
                questions=questions,
                grounded_in_rag=True
            )
        except Exception as e:
            logger.warning(f"Failed to parse LLM quiz JSON, using structured fallback: {e}")
            # Fallback single question
            fallback_q = QuizQuestion(
                id=1,
                question=f"What is a primary concept covered in {course_title}?",
                type="mcq",
                options=[
                    f"Core principles of {course_title}",
                    "Unrelated legacy theory",
                    "Random external concept",
                    "None of the above"
                ],
                correct_answer=f"Core principles of {course_title}",
                explanation=f"Based on the curriculum overview for {course_title}.",
                difficulty=difficulty,
                source_reference="Course Overview"
            )
            return QuizResult(
                course_id=course_id,
                title=f"{course_title} Review Quiz",
                total_questions=1,
                difficulty=difficulty,
                questions=[fallback_q],
                grounded_in_rag=False
            )
