"""
Specialized AI features endpoints:
Summarizer, Practice Quiz Generator, Study Planner, and Programming Tutor.
"""
from fastapi import APIRouter, Depends, Header, HTTPException
from pydantic import BaseModel, Field
from typing import List, Optional, Dict, Any
from services.quiz_generator_service import quiz_generator, QuizResult
from services.summarizer_service import summarizer, SummaryResult
from services.tutor_service import tutor_service, StudyPlanResponse
from embeddings.local_embedder import embedder
from vectorstore.qdrant_manager import qdrant
from core.security import verify_internal_api_key, parse_authorized_courses
from core.logger import get_masked_logger

logger = get_masked_logger(__name__)
router = APIRouter(prefix="/features", tags=["Features"])

# --- Quiz Request ---
class QuizGenRequest(BaseModel):
    course_id: int
    course_title: str
    topic: Optional[str] = None
    num_questions: int = 5
    difficulty: str = "Intermediate"
    question_type: str = "mcq"

# --- Summarize Request ---
class SummarizeRequest(BaseModel):
    title: str
    content: str
    summary_type: str = "revision_notes"  # "executive", "key_concepts", "revision_notes"

# --- Code Explanation Request ---
class CodeExplainRequest(BaseModel):
    code_snippet: str
    error_message: Optional[str] = None
    language: str = "csharp"
    question: Optional[str] = None

# --- Study Plan Request ---
class StudyPlanRequest(BaseModel):
    course_title: str
    completed_lessons: List[str] = Field(default_factory=list)
    incomplete_lessons: List[str] = Field(default_factory=list)
    available_days: int = 7
    daily_hours: float = 1.5

@router.post("/generate-quiz", response_model=QuizResult)
def generate_quiz(
    req: QuizGenRequest,
    api_key_valid: bool = Depends(verify_internal_api_key),
    authorized_courses_header: Optional[str] = Header(None, alias="X-Authorized-Courses")
):
    """
    Generates practice questions grounded in course vectors in Qdrant.
    """
    query_str = req.topic or req.course_title
    query_vec = embedder.embed_text(query_str)
    
    retrieved = qdrant.search_authorized_course(
        query_vector=query_vec,
        course_id=req.course_id,
        top_k=6
    )
    context_chunks = [r.get("text", "") for r in retrieved]

    return quiz_generator.generate_quiz(
        course_id=req.course_id,
        course_title=req.course_title,
        context_chunks=context_chunks,
        num_questions=req.num_questions,
        difficulty=req.difficulty,
        question_type=req.question_type,
        topic=req.topic
    )

@router.post("/summarize", response_model=SummaryResult)
def summarize(
    req: SummarizeRequest,
    api_key_valid: bool = Depends(verify_internal_api_key)
):
    """
    Summarizes learning content or generates revision notes.
    """
    return summarizer.summarize_content(
        title=req.title,
        content_text=req.content,
        summary_type=req.summary_type
    )

@router.post("/code-explain")
def explain_code(
    req: CodeExplainRequest,
    api_key_valid: bool = Depends(verify_internal_api_key)
):
    """
    Code walkthrough, debugging and error resolution.
    """
    return tutor_service.explain_code_or_error(
        code_snippet=req.code_snippet,
        error_message=req.error_message,
        language=req.language,
        question=req.question
    )

@router.post("/study-plan", response_model=StudyPlanResponse)
def generate_study_plan(
    req: StudyPlanRequest,
    api_key_valid: bool = Depends(verify_internal_api_key)
):
    """
    Creates personalized study plan tailored to student's incomplete lessons.
    """
    return tutor_service.generate_study_plan(
        course_title=req.course_title,
        completed_lessons=req.completed_lessons,
        incomplete_lessons=req.incomplete_lessons,
        available_days=req.available_days,
        daily_hours=req.daily_hours
    )
