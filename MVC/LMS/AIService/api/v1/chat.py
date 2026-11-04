"""
Chat and RAG query processing endpoints.
"""
from fastapi import APIRouter, Depends, Header, HTTPException, status
from pydantic import BaseModel, Field
from typing import List, Optional, Dict, Any
from rag.rag_pipeline import rag_pipeline
from core.security import verify_internal_api_key, parse_authorized_courses
from core.logger import get_masked_logger

logger = get_masked_logger(__name__)
router = APIRouter(prefix="/chat", tags=["Chat"])

class ChatAskRequest(BaseModel):
    user_id: str
    user_role: str = "Student"
    session_id: Optional[str] = None
    course_id: Optional[int] = None
    module_id: Optional[int] = None
    lesson_id: Optional[int] = None
    query: str
    learning_mode: str = "CourseAssistant"  # "General", "CourseAssistant", "ExamPrep", "PracticeQuiz", "CodeTutor"
    history: Optional[List[Dict[str, str]]] = Field(default_factory=list)

class SourceCitation(BaseModel):
    source_index: int
    document_id: Optional[int] = None
    chunk_id: Optional[str] = None
    source_title: str
    page_number: Optional[int] = None
    relevance_score: float = 0.0
    excerpt: str = ""

class ChatAskResponse(BaseModel):
    answer: str
    sources: List[SourceCitation]
    grounded_in_rag: bool
    learning_mode: str
    tokens_used: int
    llm_provider: str
    model_used: str
    failover_occurred: bool = False
    duration_ms: int = 0

@router.post("/ask", response_model=ChatAskResponse)
def ask_chat(
    req: ChatAskRequest,
    api_key_valid: bool = Depends(verify_internal_api_key),
    authorized_courses_header: Optional[str] = Header(None, alias="X-Authorized-Courses")
):
    """
    RAG-grounded query processing endpoint.
    Enforces course isolation: students may only retrieve authorized courses.
    """
    authorized_courses = parse_authorized_courses(authorized_courses_header)
    
    # Check enrollment authorization for course-specific questions
    if req.course_id and req.user_role == "Student":
        if authorized_courses is not None and req.course_id not in authorized_courses:
            logger.warning(f"Unauthorized RAG query attempt by user {req.user_id} for course {req.course_id}")
            raise HTTPException(
                status_code=status.HTTP_403_FORBIDDEN,
                detail=f"Access denied: You are not enrolled in course {req.course_id}."
            )

    logger.info(f"Processing query from user {req.user_id} (mode={req.learning_mode}, course={req.course_id})")

    result = rag_pipeline.answer_query(
        query=req.query,
        course_id=req.course_id,
        mode=req.learning_mode,
        chat_history=req.history
    )

    citations = [
        SourceCitation(
            source_index=s.get("source_index", idx + 1),
            document_id=s.get("document_id"),
            chunk_id=s.get("chunk_id"),
            source_title=s.get("source_title", "Course Material"),
            page_number=s.get("page_number"),
            relevance_score=s.get("relevance_score", 0.0),
            excerpt=s.get("excerpt", "")
        ) for idx, s in enumerate(result.get("sources", []))
    ]

    return ChatAskResponse(
        answer=result.get("answer", ""),
        sources=citations,
        grounded_in_rag=len(citations) > 0,
        learning_mode=req.learning_mode,
        tokens_used=result.get("total_tokens", 0),
        llm_provider=result.get("provider_used", ""),
        model_used=result.get("model_used", ""),
        failover_occurred=result.get("failover_occurred", False),
        duration_ms=result.get("duration_ms", 0)
    )
