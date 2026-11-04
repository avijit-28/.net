"""
Specialized AI features endpoints:
Summarizer, Practice Quiz Generator, Study Planner, and Programming Tutor.
"""
from fastapi import APIRouter, Depends, Header, HTTPException
from pydantic import BaseModel, Field
from typing import List, Optional, Dict, Any
from services.quiz_generator_service import QuizGeneratorService, QuizResult
from services.summarizer_service import SummarizerService, SummaryResult
from services.tutor_service import TutorService, StudyPlanResponse
from vectorstore.qdrant_manager import QdrantManager
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
async def generate_quiz(
    req: QuizGenRequest,
    api_key_valid: bool = Depends(verify_internal_api_key),
    authorized_courses_header: Optional[str] = Header(None, alias="X-Authorized-Courses")
):
    """
    Generates practice questions grounded in course vectors in Qdrant.
    """
    qdrant = QdrantManager.get_instance()
    # Retrieve top chunks for this course/topic to ground quiz
    retrieved = qdrant.search_by_text(
        query=req.topic or req.course_title,
        course_id=req.course_id,
        limit=6
    )
    context_chunks = [r["text"] for r in retrieved]

    service = QuizGeneratorService()
    result = await service.generate_quiz(
        course_id=req.course_id,
        course_title=req.course_title,
        context_chunks=context_chunks,
        num_questions=req.num_questions,
        difficulty=req.difficulty,
        question_type=req.question_type,
        topic=req.topic
    )
    return result

@router.post("/summarize", response_model=SummaryResult)
async def summarize(
    req: SummarizeRequest,
    api_key_valid: bool = Depends(verify_internal_api_key)
):
    """
    Summarizes learning content or generates revision notes.
    """
    service = SummarizerService()
    return await service.summarize_content(
        title=req.title,
        content_text=req.content,
        summary_type=req.summary_type
    )

@router.post("/code-explain")
async def explain_code(
    req: CodeExplainRequest,
    api_key_valid: bool = Depends(verify_internal_api_key)
):
    """
    Code walkthrough, debugging and error resolution.
    """
    service = TutorService()
    return await service.explain_code_or_error(
        code_snippet=req.code_snippet,
        error_message=req.error_message,
        language=req.language,
        question=req.question
    )

@router.post("/study-plan", response_model=StudyPlanResponse)
async def generate_study_plan(
    req: StudyPlanRequest,
    api_key_valid: bool = Depends(verify_internal_api_key)
):
    """
    Creates personalized study plan tailored to student's incomplete lessons.
    """
    service = TutorService()
    return await service.generate_study_plan(
        course_title=req.course_title,
        completed_lessons=req.completed_lessons,
        incomplete_lessons=req.incomplete_lessons,
        available_days=req.available_days,
        daily_hours=req.daily_hours
    )
