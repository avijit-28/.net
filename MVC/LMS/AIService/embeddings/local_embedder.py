"""
EduPlatform AIService - Local Sentence Transformers Embeddings
Produces local vector embeddings without calling external paid APIs.
"""

from typing import List
import numpy as np
from config import settings
from core.logger import logger

class LocalEmbedder:
    _instance = None
    _model = None

    def __new__(cls):
        if cls._instance is None:
            cls._instance = super(LocalEmbedder, cls).__new__(cls)
            cls._instance._init_model()
        return cls._instance

    def _init_model(self):
        try:
            from sentence_transformers import SentenceTransformer
            logger.info(f"Loading local embedding model: {settings.EMBEDDING_MODEL}...")
            self._model = SentenceTransformer(settings.EMBEDDING_MODEL)
            logger.info("Local SentenceTransformer successfully loaded.")
        except Exception as e:
            logger.warning(f"SentenceTransformer not yet available: {e}. Utilizing lightweight vectorizer.")
            self._model = None

    @property
    def dimension(self) -> int:
        return settings.EMBEDDING_DIMENSION

    def embed_text(self, text: str) -> List[float]:
        """Generates a dense vector for a single query."""
        if self._model:
            emb = self._model.encode(text, normalize_embeddings=True)
            return emb.tolist()
        return self._fallback_embed(text)

    def embed_documents(self, texts: List[str]) -> List[List[float]]:
        """Generates embeddings for a batch of documents."""
        if not texts:
            return []
        if self._model:
            embs = self._model.encode(texts, batch_size=32, normalize_embeddings=True)
            return embs.tolist()
        return [self._fallback_embed(t) for t in texts]

    def _fallback_embed(self, text: str) -> List[float]:
        """Deterministic 384-dimensional normalized vector for zero-config fallback."""
        dim = settings.EMBEDDING_DIMENSION
        vec = np.zeros(dim, dtype=np.float32)
        tokens = text.lower().split()
        for i, token in enumerate(tokens):
            h = hash(token) % dim
            vec[h] += 1.0 / (i + 1)
        norm = np.linalg.norm(vec)
        if norm > 0:
            vec = vec / norm
        return vec.tolist()

embedder = LocalEmbedder()
