"""
Quiz Generator Service for RAG AI Learning Assistant.
Generates structured practice quizzes (MCQs, True/False, Short Answer)
grounded in retrieved course materials or lesson contents.
"""
import json
import re
from typing import List, Dict, Any, Optional
from pydantic import BaseModel, Field
from llm.provider_factory import llm_factory
from core.logger import get_masked_logger

logger = get_masked_logger(__name__)

QUIZ_SYSTEM_PROMPT = """You are an expert pedagogical examination designer at EduPlatform LMS.
Generate clear, rigorous, educational practice quiz questions strictly grounded in the provided course materials.
Always provide the correct answer and a pedagogical explanation."""

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
    def generate_quiz(
        self,
        course_id: int,
        course_title: str,
        context_chunks: List[str],
        num_questions: int = 5,
        difficulty: str = "Intermediate",
        question_type: str = "mcq",
        topic: Optional[str] = None
    ) -> QuizResult:
        logger.info(f"Generating practice quiz for course {course_id} ({num_questions} questions, {difficulty})")
        
        context_text = "\n\n---\n\n".join(context_chunks)
        if not context_text.strip():
            context_text = f"Course: {course_title}. Core principles and fundamental concepts of {topic or course_title}."

        prompt = f"""Generate a {difficulty}-level practice quiz of {num_questions} questions on: {topic or course_title}.
Question type: {question_type}.

GROUNDING COURSE MATERIALS:
{context_text[:8000]}

INSTRUCTIONS:
1. Every question must be directly grounded in the provided material.
2. Return ONLY a valid JSON object matching this exact schema:
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
      "explanation": "Why Option A is correct based on the course materials.",
      "difficulty": "{difficulty}",
      "source_reference": "Section or concept reference"
    }}
  ]
}}
Do not include any introductory remarks, markdown fences, or conversational padding. Return raw JSON only."""

        res = llm_factory.generate(
            prompt=prompt,
            system_instruction=QUIZ_SYSTEM_PROMPT,
            temperature=0.3
        )
        response_text = res.get("text", "")

        try:
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
            logger.warning(f"Failed to parse quiz JSON from LLM: {e}")
            fallback_q = QuizQuestion(
                id=1,
                question=f"Which core concept is central to {course_title}?",
                type="mcq",
                options=[
                    f"Core principles of {course_title}",
                    "External unrelated framework",
                    "Deprecated legacy standard",
                    "None of the above"
                ],
                correct_answer=f"Core principles of {course_title}",
                explanation=f"Based on the foundational curriculum structure of {course_title}.",
                difficulty=difficulty,
                source_reference="Course Overview"
            )
            return QuizResult(
                course_id=course_id,
                title=f"{course_title} Practice Review",
                total_questions=1,
                difficulty=difficulty,
                questions=[fallback_q],
                grounded_in_rag=False
            )

quiz_generator = QuizGeneratorService()
