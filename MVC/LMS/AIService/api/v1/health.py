"""
Health and telemetry endpoint for AIService.
"""
from fastapi import APIRouter
from vectorstore.qdrant_manager import QdrantManager
from embeddings.local_embedder import LocalEmbedder
from config import settings
import os

router = APIRouter(prefix="/health", tags=["Health"])

@router.get("")
async def get_health():
    """
    Returns live health of vectorstore, embedding engine, and configuration.
    """
    qdrant = QdrantManager.get_instance()
    count = qdrant.count()

    return {
        "status": "healthy",
        "service": settings.APP_NAME,
        "environment": settings.ENVIRONMENT,
        "vector_db": {
            "mode": "embedded_disk" if not settings.QDRANT_URL else "remote",
            "collection": settings.QDRANT_COLLECTION,
            "indexed_vectors_count": count
        },
        "embedding_model": {
            "model_name": settings.EMBEDDING_MODEL_NAME,
            "dimension": settings.EMBEDDING_DIM
        },
        "llm": {
            "primary": settings.DEFAULT_LLM_PROVIDER,
            "failover": settings.FAILOVER_LLM_PROVIDER,
            "gemini_configured": bool(settings.GEMINI_API_KEY),
            "groq_configured": bool(settings.GROQ_API_KEY)
        }
    }
