"""
EduPlatform AIService - Qdrant Vector Database Manager
Supports both remote Qdrant servers (Docker/Cloud) and local embedded on-disk storage.
Enforces strict course authorization filters on every similarity search.
"""

from typing import List, Dict, Any, Optional
from pathlib import Path
from qdrant_client import QdrantClient
from qdrant_client.models import (
    Distance,
    VectorParams,
    PointStruct,
    Filter,
    FieldCondition,
    MatchValue,
    PayloadSchemaType
)
from config import settings
from core.logger import logger
from core.exceptions import VectorStoreError

class QdrantManager:
    _instance = None

    def __new__(cls):
        if cls._instance is None:
            cls._instance = super(QdrantManager, cls).__new__(cls)
            cls._instance._init_client()
        return cls._instance

    def _init_client(self):
        self.collection_name = settings.QDRANT_COLLECTION
        try:
            if settings.QDRANT_URL and settings.QDRANT_URL.startswith("http"):
                logger.info(f"Connecting to remote Qdrant at {settings.QDRANT_URL}...")
                self.client = QdrantClient(
                    url=settings.QDRANT_URL,
                    api_key=settings.QDRANT_API_KEY or None,
                    timeout=5
                )
                # Test connection
                self.client.get_collections()
                logger.info("Connected to remote Qdrant server.")
            else:
                raise ValueError("No remote Qdrant URL specified.")
        except Exception as e:
            logger.info(f"Remote Qdrant unavailable ({e}). Initializing embedded local on-disk storage at '{settings.QDRANT_STORAGE_PATH}'...")
            storage_path = Path(settings.QDRANT_STORAGE_PATH)
            storage_path.mkdir(parents=True, exist_ok=True)
            self.client = QdrantClient(path=str(storage_path))
            logger.info("Embedded local Qdrant successfully initialized.")

        self._ensure_collection()

    def _ensure_collection(self):
        try:
            collections = self.client.get_collections().collections
            exists = any(c.name == self.collection_name for c in collections)
            if not exists:
                logger.info(f"Creating Qdrant collection '{self.collection_name}' (dim={settings.EMBEDDING_DIMENSION}, Cosine)...")
                self.client.create_collection(
                    collection_name=self.collection_name,
                    vectors_config=VectorParams(
                        size=settings.EMBEDDING_DIMENSION,
                        distance=Distance.COSINE
                    )
                )
                # Create payload indexes for fast authorization filtering
                self.client.create_payload_index(
                    collection_name=self.collection_name,
                    field_name="course_id",
                    field_schema=PayloadSchemaType.INTEGER
                )
                self.client.create_payload_index(
                    collection_name=self.collection_name,
                    field_name="document_id",
                    field_schema=PayloadSchemaType.INTEGER
                )
                logger.info(f"Collection '{self.collection_name}' and indexes created successfully.")
        except Exception as ex:
            logger.error(f"Error ensuring Qdrant collection: {ex}")

    def upsert_chunks(self, chunks: List[Dict[str, Any]], vectors: List[List[float]]) -> int:
        """Stores vectors and full metadata payload in Qdrant."""
        if not chunks or not vectors or len(chunks) != len(vectors):
            return 0

        points: List[PointStruct] = []
        for chunk, vector in zip(chunks, vectors):
            points.append(PointStruct(
                id=chunk["chunk_id"],
                vector=vector,
                payload={
                    "document_id": chunk["document_id"],
                    "course_id": chunk["course_id"],
                    "module_id": chunk.get("module_id"),
                    "lesson_id": chunk.get("lesson_id"),
                    "page_number": chunk.get("page_number", 1),
                    "section_title": chunk.get("section_title", ""),
                    "chunk_index": chunk["chunk_index"],
                    "text": chunk["text"]
                }
            ))

        try:
            self.client.upsert(
                collection_name=self.collection_name,
                points=points
            )
            logger.info(f"Upserted {len(points)} vectors into Qdrant collection '{self.collection_name}'.")
            return len(points)
        except Exception as ex:
            logger.error(f"Failed to upsert points into Qdrant: {ex}")
            raise VectorStoreError(f"Qdrant upsert failed: {str(ex)}")

    def search_authorized_course(
        self,
        query_vector: List[float],
        course_id: Optional[int],
        top_k: int = 5,
        threshold: float = 0.45
    ) -> List[Dict[str, Any]]:
        """
        Performs semantic vector search with strict course authorization filtering.
        If course_id is provided, only chunks belonging to that exact course are returned.
        """
        query_filter = None
        if course_id is not None:
            query_filter = Filter(
                must=[
                    FieldCondition(
                        key="course_id",
                        match=MatchValue(value=int(course_id))
                    )
                ]
            )

        try:
            # Execute vector search
            hits = self.client.search(
                collection_name=self.collection_name,
                query_vector=query_vector,
                query_filter=query_filter,
                limit=top_k,
                score_threshold=threshold
            )

            results: List[Dict[str, Any]] = []
            for hit in hits:
                payload = hit.payload or {}
                results.append({
                    "chunk_id": str(hit.id),
                    "score": float(hit.score),
                    "text": payload.get("text", ""),
                    "document_id": payload.get("document_id"),
                    "course_id": payload.get("course_id"),
                    "module_id": payload.get("module_id"),
                    "lesson_id": payload.get("lesson_id"),
                    "page_number": payload.get("page_number"),
                    "section_title": payload.get("section_title")
                })

            return results
        except Exception as ex:
            logger.error(f"Qdrant vector search failed: {ex}")
            return []

    def delete_document_vectors(self, document_id: int) -> bool:
        """Deletes all chunks belonging to a deleted or re-indexed document."""
        try:
            doc_filter = Filter(
                must=[
                    FieldCondition(
                        key="document_id",
                        match=MatchValue(value=int(document_id))
                    )
                ]
            )
            self.client.delete(
                collection_name=self.collection_name,
                points_selector=doc_filter
            )
            logger.info(f"Deleted vectors for document_id={document_id} from Qdrant.")
            return True
        except Exception as ex:
            logger.error(f"Failed to delete document vectors from Qdrant: {ex}")
            return False

qdrant = QdrantManager()
