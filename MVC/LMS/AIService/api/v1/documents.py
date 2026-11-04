"""
Document ingestion and lifecycle management endpoints.
Supports PDF, DOCX, PPTX, and TXT files.
"""
from fastapi import APIRouter, Depends, Header, HTTPException, status
from pydantic import BaseModel
from typing import Optional, Dict, Any
from pathlib import Path
from services.document_service import document_service
from core.security import verify_internal_api_key
from core.logger import get_masked_logger
import os

logger = get_masked_logger(__name__)
router = APIRouter(prefix="/documents", tags=["Documents"])

class IndexRequest(BaseModel):
    document_id: int
    course_id: int
    module_id: Optional[int] = None
    lesson_id: Optional[int] = None
    title: str
    file_path: str
    access_scope: str = "enrolled_students"

class IndexResponse(BaseModel):
    status: str
    document_id: int
    chunks_created: int
    message: str

@router.post("/index", response_model=IndexResponse)
def index_document(
    req: IndexRequest,
    api_key_valid: bool = Depends(verify_internal_api_key)
):
    """
    Indexes an uploaded document into Qdrant.
    """
    logger.info(f"Received indexing request for document {req.document_id} ('{req.title}')")
    
    path = Path(req.file_path)
    if not path.exists():
        raise HTTPException(status_code=404, detail=f"File not found on server: {req.file_path}")

    file_ext = path.suffix.lower().replace(".", "") or "txt"
    try:
        result = document_service.index_document(
            document_id=req.document_id,
            course_id=req.course_id,
            file_path=str(path),
            file_type=file_ext,
            module_id=req.module_id,
            lesson_id=req.lesson_id
        )
        return IndexResponse(
            status=result["status"],
            document_id=req.document_id,
            chunks_created=result["total_chunks"],
            message="Document successfully processed and indexed into Qdrant vector database."
        )
    except Exception as e:
        logger.error(f"Error during document indexing: {e}")
        raise HTTPException(status_code=500, detail=str(e))

@router.post("/{document_id}/reindex", response_model=IndexResponse)
def reindex_document(
    document_id: int,
    req: IndexRequest,
    api_key_valid: bool = Depends(verify_internal_api_key)
):
    """
    Deletes existing vectors for document and re-indexes the file.
    """
    path = Path(req.file_path)
    if not path.exists():
        raise HTTPException(status_code=404, detail=f"File not found on server: {req.file_path}")

    file_ext = path.suffix.lower().replace(".", "") or "txt"
    try:
        result = document_service.index_document(
            document_id=document_id,
            course_id=req.course_id,
            file_path=str(path),
            file_type=file_ext,
            module_id=req.module_id,
            lesson_id=req.lesson_id
        )
        return IndexResponse(
            status="reindexed",
            document_id=document_id,
            chunks_created=result["total_chunks"],
            message="Document successfully re-indexed into Qdrant."
        )
    except Exception as e:
        logger.error(f"Error during document reindexing: {e}")
        raise HTTPException(status_code=500, detail=str(e))

@router.delete("/{document_id}")
def delete_document(
    document_id: int,
    api_key_valid: bool = Depends(verify_internal_api_key)
):
    """
    Removes all vector embeddings and chunk metadata for the specified document.
    """
    success = document_service.delete_document(document_id)
    return {
        "status": "deleted" if success else "not_found",
        "document_id": document_id,
        "message": f"Successfully deleted all vector representations for document {document_id} from Qdrant."
    }
