"""
EduPlatform AIService - Configuration Management
Loads settings from .env using Pydantic Settings.
"""

import os
from pathlib import Path
from pydantic_settings import BaseSettings
from pydantic import Field

BASE_DIR = Path(__file__).resolve().parent

class Settings(BaseSettings):
    # LLM Settings
    GEMINI_API_KEY: str = Field(default="", env="GEMINI_API_KEY")
    GEMINI_MODEL: str = Field(default="gemini-1.5-flash", env="GEMINI_MODEL")

    GROQ_API_KEY: str = Field(default="", env="GROQ_API_KEY")
    GROQ_MODEL: str = Field(default="llama-3.1-8b-instant", env="GROQ_MODEL")

    DEFAULT_LLM_PROVIDER: str = Field(default="gemini", env="DEFAULT_LLM_PROVIDER")

    # Local Embeddings
    EMBEDDING_MODEL: str = Field(default="BAAI/bge-small-en-v1.5", env="EMBEDDING_MODEL")
    EMBEDDING_DIMENSION: int = 384

    # Vector Store (Qdrant)
    QDRANT_URL: str = Field(default="", env="QDRANT_URL")
    QDRANT_API_KEY: str = Field(default="", env="QDRANT_API_KEY")
    QDRANT_STORAGE_PATH: str = Field(default=str(BASE_DIR / "qdrant_data"), env="QDRANT_STORAGE_PATH")
    QDRANT_COLLECTION: str = Field(default="lms_course_knowledge", env="QDRANT_COLLECTION")

    # Security
    AISERVICE_INTERNAL_KEY: str = Field(default="lms_rag_secret_auth_token_2026_xyz987", env="AISERVICE_INTERNAL_KEY")

    # RAG Hyperparameters
    TOP_K: int = Field(default=5, env="TOP_K")
    CHUNK_SIZE: int = Field(default=1000, env="CHUNK_SIZE")
    CHUNK_OVERLAP: int = Field(default=200, env="CHUNK_OVERLAP")
    SIMILARITY_THRESHOLD: float = Field(default=0.50, env="SIMILARITY_THRESHOLD")
    TEMPERATURE: float = Field(default=0.2, env="TEMPERATURE")
    MAX_OUTPUT_TOKENS: int = Field(default=1024, env="MAX_OUTPUT_TOKENS")

    # Storage paths
    UPLOAD_DIR: str = Field(default=str(BASE_DIR / "uploads"))

    # Server Settings
    HOST: str = "127.0.0.1"
    PORT: int = 8000

    class Config:
        env_file = str(BASE_DIR / ".env")
        env_file_encoding = "utf-8"
        extra = "ignore"

settings = Settings()
