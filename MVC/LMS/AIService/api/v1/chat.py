"""
Chat and RAG query processing endpoints.
"""
from fastapi import APIRouter, Depends, Header, HTTPException, status
from pydantic import BaseModel, Field
from typing import List, Optional, Dict, Any
from rag.rag_pipeline import RAGPipeline
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
    learning_mode: str = "general"  # "general", "course", "exam", "quiz", "code"
    history: Optional[List[Dict[str, str]]] = Field(default_factory=list)

class SourceCitation(BaseModel):
    chunk_id: str
    document_id: int
    course_id: int
    module_id: Optional[int] = None
    lesson_id: Optional[int] = None
    document_title: str
    page_number: Optional[int] = None
    section_name: Optional[str] = None
    text_snippet: str
    score: float

class ChatAskResponse(BaseModel):
    answer: str
    sources: List[SourceCitation]
    grounded_in_rag: bool
    learning_mode: str
    tokens_used: int
    llm_provider: str

@router.post("/ask", response_model=ChatAskResponse)
async def ask_chat(
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

    pipeline = RAGPipeline()
    result = await pipeline.query(
        user_query=req.query,
        course_id=req.course_id,
        module_id=req.module_id,
        lesson_id=req.lesson_id,
        authorized_course_ids=authorized_courses,
        learning_mode=req.learning_mode,
        history=req.history
    )

    citations = [
        SourceCitation(
            chunk_id=s["chunk_id"],
            document_id=s["document_id"],
            course_id=s["course_id"],
            module_id=s.get("module_id"),
            lesson_id=s.get("lesson_id"),
            document_title=s["document_title"],
            page_number=s.get("page_number"),
            section_name=s.get("section_name"),
            text_snippet=s["text_snippet"],
            score=s["score"]
        ) for s in result["sources"]
    ]

    return ChatAskResponse(
        answer=result["answer"],
        sources=citations,
        grounded_in_rag=result["grounded_in_rag"],
        learning_mode=req.learning_mode,
        tokens_used=result["tokens_used"],
        llm_provider=result["llm_provider"]
    )
