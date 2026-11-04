"""
EduPlatform AIService - Document Ingestion & Processing Service
Coordinates extraction, chunking, local vector embedding, and Qdrant indexing.
"""

from pathlib import Path
from typing import Dict, Any, List
from config import settings
from core.logger import logger
from core.exceptions import DocumentExtractionError
from extractors.pdf_extractor import PdfExtractor
from extractors.docx_extractor import DocxExtractor
from extractors.pptx_extractor import PptxExtractor
from extractors.text_cleaner import clean_text
from rag.chunker import RecursiveChunker
from embeddings.local_embedder import embedder
from vectorstore.qdrant_manager import qdrant

class DocumentService:
    def __init__(self):
        self.chunker = RecursiveChunker()

    def index_document(
        self,
        document_id: int,
        course_id: int,
        file_path: str,
        file_type: str,
        module_id: int = None,
        lesson_id: int = None
    ) -> Dict[str, Any]:
        """
        Executes full ingestion pipeline for an uploaded course document:
        1. Extract text pages
        2. Chunk text
        3. Generate local vector embeddings
        4. Upsert into Qdrant collection with course metadata
        """
        path = Path(file_path)
        if not path.exists():
            raise DocumentExtractionError(f"File not found on disk: {file_path}")

        ft = file_type.upper().replace(".", "")
        logger.info(f"Beginning ingestion for Document #{document_id} ({ft}) of Course #{course_id}...")

        # 1. First remove any previous vectors for this document (idempotent re-indexing)
        qdrant.delete_document_vectors(document_id)

        # 2. Extract pages/sections
        pages: List[Dict[str, Any]] = []
        if ft == "PDF":
            pages = PdfExtractor.extract(str(path))
        elif ft in ["DOCX", "DOC"]:
            pages = DocxExtractor.extract(str(path))
        elif ft in ["PPTX", "PPT"]:
            pages = PptxExtractor.extract(str(path))
        elif ft in ["TXT", "MD"]:
            with open(path, "r", encoding="utf-8", errors="ignore") as f:
                content = clean_text(f.read())
            pages = [{"page_number": 1, "section_title": path.stem, "text": content}]
        else:
            raise DocumentExtractionError(f"Unsupported document format: {file_type}")

        if not pages:
            raise DocumentExtractionError(f"No readable content could be extracted from {path.name}")

        # 3. Chunk text with metadata
        chunks = self.chunker.chunk_document_pages(
            pages=pages,
            document_id=document_id,
            course_id=course_id,
            module_id=module_id,
            lesson_id=lesson_id
        )

        if not chunks:
            raise DocumentExtractionError(f"Document {path.name} produced 0 chunks after processing.")

        # 4. Generate dense embeddings locally
        texts = [c["text"] for c in chunks]
        vectors = embedder.embed_documents(texts)

        # 5. Store in Qdrant
        upserted_count = qdrant.upsert_chunks(chunks, vectors)

        logger.info(f"Successfully indexed Document #{document_id}: {len(pages)} pages, {upserted_count} vector chunks.")

        return {
            "document_id": document_id,
            "course_id": course_id,
            "file_name": path.name,
            "total_pages": len(pages),
            "total_chunks": upserted_count,
            "status": "Indexed"
        }

    def delete_document(self, document_id: int) -> bool:
        """Removes all vector embeddings for the specified document."""
        return qdrant.delete_document_vectors(document_id)

document_service = DocumentService()
