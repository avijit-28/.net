"""
Document ingestion and lifecycle management endpoints.
Supports PDF, DOCX, PPTX, and TXT files.
"""
from fastapi import APIRouter, Depends, Header, HTTPException, UploadFile, File, Form, status, BackgroundTasks
from pydantic import BaseModel
from typing import Optional, Dict, Any
from services.document_service import DocumentService
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
async def index_document(
    req: IndexRequest,
    api_key_valid: bool = Depends(verify_internal_api_key)
):
    """
    Synchronously or background-indexes a document into Qdrant.
    """
    logger.info(f"Received indexing request for document {req.document_id} ('{req.title}')")
    
    if not os.path.exists(req.file_path):
        raise HTTPException(status_code=404, detail=f"File not found on server: {req.file_path}")

    service = DocumentService()
    try:
        result = await service.process_and_index_document(
            document_id=req.document_id,
            course_id=req.course_id,
            module_id=req.module_id,
            lesson_id=req.lesson_id,
            title=req.title,
            file_path=req.file_path,
            access_scope=req.access_scope
        )
        return IndexResponse(
            status=result["status"],
            document_id=req.document_id,
            chunks_created=result["chunks_indexed"],
            message="Document successfully processed and indexed into Qdrant vector database."
        )
    except Exception as e:
        logger.error(f"Error during document indexing: {e}")
        raise HTTPException(status_code=500, detail=str(e))

@router.post("/{document_id}/reindex", response_model=IndexResponse)
async def reindex_document(
    document_id: int,
    req: IndexRequest,
    api_key_valid: bool = Depends(verify_internal_api_key)
):
    """
    Deletes existing vectors for document and re-indexes the file.
    """
    service = DocumentService()
    # Delete old vectors
    service.delete_document_vectors(document_id)
    # Re-index
    result = await service.process_and_index_document(
        document_id=document_id,
        course_id=req.course_id,
        module_id=req.module_id,
        lesson_id=req.lesson_id,
        title=req.title,
        file_path=req.file_path,
        access_scope=req.access_scope
    )
    return IndexResponse(
        status="reindexed",
        document_id=document_id,
        chunks_created=result["chunks_indexed"],
        message="Document successfully re-indexed into Qdrant."
    )

@router.delete("/{document_id}")
async def delete_document(
    document_id: int,
    api_key_valid: bool = Depends(verify_internal_api_key)
):
    """
    Removes all vector embeddings and chunk metadata for the specified document.
    """
    service = DocumentService()
    service.delete_document_vectors(document_id)
    return {
        "status": "deleted",
        "document_id": document_id,
        "message": f"Successfully deleted all vector representations for document {document_id} from Qdrant."
    }
