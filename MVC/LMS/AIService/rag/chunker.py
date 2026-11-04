"""
EduPlatform AIService - Text Chunking & Metadata Attachment
Splits document pages/sections into semantically coherent overlapping chunks.
"""

import uuid
from typing import List, Dict, Any
from config import settings

class RecursiveChunker:
    def __init__(self, chunk_size: int = None, chunk_overlap: int = None):
        self.chunk_size = chunk_size or settings.CHUNK_SIZE
        self.chunk_overlap = chunk_overlap or settings.CHUNK_OVERLAP
        self.separators = ["\n\n", "\n", ". ", "? ", "! ", " ", ""]

    def chunk_document_pages(
        self,
        pages: List[Dict[str, Any]],
        document_id: int,
        course_id: int,
        module_id: int = None,
        lesson_id: int = None
    ) -> List[Dict[str, Any]]:
        """
        Takes raw page dictionaries, splits text into chunks, and attaches metadata.
        """
        all_chunks: List[Dict[str, Any]] = []
        global_chunk_idx = 0

        for page in pages:
            page_text = page.get("text", "").strip()
            page_num = page.get("page_number", 1)
            section_title = page.get("section_title") or f"Page {page_num}"

            if not page_text or len(page_text) < 15:
                continue

            raw_chunks = self._split_text(page_text, self.chunk_size, self.chunk_overlap)

            for chunk_str in raw_chunks:
                chunk_str = chunk_str.strip()
                if len(chunk_str) < 15:
                    continue

                # Generate a deterministic vector UUID based on doc and chunk index
                point_id = str(uuid.uuid5(uuid.NAMESPACE_DNS, f"doc_{document_id}_chunk_{global_chunk_idx}"))

                all_chunks.append({
                    "chunk_id": point_id,
                    "chunk_index": global_chunk_idx,
                    "document_id": document_id,
                    "course_id": course_id,
                    "module_id": module_id,
                    "lesson_id": lesson_id,
                    "page_number": page_num,
                    "section_title": section_title,
                    "text": chunk_str
                })
                global_chunk_idx += 1

        return all_chunks

    def _split_text(self, text: str, chunk_size: int, chunk_overlap: int) -> List[str]:
        """Recursive splitting by paragraphs, sentences, and words."""
        if len(text) <= chunk_size:
            return [text]

        chunks = []
        start = 0
        text_len = len(text)

        while start < text_len:
            end = start + chunk_size
            if end >= text_len:
                chunks.append(text[start:].strip())
                break

            # Find best split separator before end
            split_pos = -1
            for sep in self.separators:
                pos = text.rfind(sep, start, end)
                if pos != -1 and pos > start:
                    split_pos = pos + len(sep)
                    break

            if split_pos == -1 or split_pos <= start:
                split_pos = end

            chunk = text[start:split_pos].strip()
            if chunk:
                chunks.append(chunk)

            start = split_pos - chunk_overlap
            if start < 0 or start >= split_pos:
                start = split_pos

        return chunks
