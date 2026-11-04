"""
EduPlatform LMS - RAG AI Learning Assistant Service
FastAPI Independent Microservice Entrypoint.
"""
import uvicorn
from contextlib import asynccontextmanager
from fastapi import FastAPI, Request, status
from fastapi.responses import JSONResponse
from fastapi.middleware.cors import CORSMiddleware
from config import settings
from core.logger import get_masked_logger
from vectorstore.qdrant_manager import QdrantManager
from embeddings.local_embedder import LocalEmbedder
from api.v1.health import router as health_router
from api.v1.chat import router as chat_router
from api.v1.documents import router as documents_router
from api.v1.features import router as features_router

logger = get_masked_logger(__name__)

@asynccontextmanager
async def lifespan(app: FastAPI):
    # Startup sequence
    logger.info("=" * 60)
    logger.info(f"Starting {settings.APP_NAME} v{settings.VERSION} [{settings.ENVIRONMENT}]")
    logger.info(f"Primary LLM Provider: {settings.DEFAULT_LLM_PROVIDER} | Failover: {settings.FAILOVER_LLM_PROVIDER}")
    
    # Pre-warm local embedder and Qdrant collection
    try:
        dim = embedder.dimension
        logger.info(f"Embedding Engine loaded: {settings.EMBEDDING_MODEL_NAME} (dim={dim})")
        vector_count = qdrant.get_collection_vector_count()
        logger.info(f"Qdrant vectorstore initialized with {vector_count} indexed chunks.")
    except Exception as e:
        logger.error(f"Error during pre-warming services: {e}")
        
    logger.info("=" * 60)
    yield
    # Shutdown sequence
    logger.info(f"Shutting down {settings.APP_NAME} gracefully.")

app = FastAPI(
    title=settings.APP_NAME,
    version=settings.VERSION,
    description="Course-aware RAG AI Learning Assistant for ASP.NET MVC 5 LMS",
    lifespan=lifespan
)

# CORS Middleware allowing ASP.NET MVC origin
app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:56102", "http://127.0.0.1:56102", "http://localhost:*", "*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Global Exception Handler
@app.exception_handler(Exception)
async def global_exception_handler(request: Request, exc: Exception):
    logger.error(f"Unhandled exception on {request.url.path}: {str(exc)}")
    return JSONResponse(
        status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
        content={
            "error": "InternalServerError",
            "message": "An unexpected error occurred in the AI Learning Assistant service.",
            "path": request.url.path
        }
    )

# Include Routers under /api/v1
app.include_router(health_router, prefix="/api/v1")
app.include_router(chat_router, prefix="/api/v1")
app.include_router(documents_router, prefix="/api/v1")
app.include_router(features_router, prefix="/api/v1")

@app.get("/")
async def root():
    return {
        "service": settings.APP_NAME,
        "version": settings.VERSION,
        "docs_url": "/docs",
        "health_check": "/api/v1/health"
    }

if __name__ == "__main__":
    uvicorn.run(
        "main:app",
        host=settings.HOST,
        port=settings.PORT,
        reload=settings.DEBUG,
        log_level=settings.LOG_LEVEL.lower()
    )
