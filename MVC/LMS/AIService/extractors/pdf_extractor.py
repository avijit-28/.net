"""
EduPlatform AIService - PyMuPDF Page-by-Page PDF Extractor
Extracts text while strictly preserving page numbers and document structure.
"""

from pathlib import Path
from typing import List, Dict, Any
import fitz  # PyMuPDF
from extractors.text_cleaner import clean_text, is_text_meaningful
from core.logger import logger
from core.exceptions import DocumentExtractionError

class PdfExtractor:
    @staticmethod
    def extract(file_path: str) -> List[Dict[str, Any]]:
        path = Path(file_path)
        if not path.exists():
            raise DocumentExtractionError(f"File not found: {file_path}")

        pages: List[Dict[str, Any]] = []
        try:
            doc = fitz.open(file_path)
            total_pages = len(doc)
            logger.info(f"Extracting PDF '{path.name}' ({total_pages} pages)...")

            for page_idx in range(total_pages):
                page = doc[page_idx]
                page_num = page_idx + 1
                raw_text = page.get_text("text") or ""
                cleaned = clean_text(raw_text)

                # Check if page is empty / scanned image
                is_scanned = False
                if not is_text_meaningful(cleaned, min_chars=15):
                    # Check if page contains images
                    images = page.get_images()
                    if images:
                        is_scanned = True
                        cleaned = f"[Scanned page {page_num}: image content present without embedded text]"

                pages.append({
                    "page_number": page_num,
                    "text": cleaned,
                    "is_scanned": is_scanned,
                    "char_count": len(cleaned)
                })

            doc.close()
        except Exception as ex:
            logger.error(f"Failed to extract PDF '{path.name}': {ex}")
            raise DocumentExtractionError(f"Error extracting PDF: {str(ex)}")

        # Verify at least one page had content
        non_empty = [p for p in pages if is_text_meaningful(p["text"]) and not p["is_scanned"]]
        if not non_empty and total_pages > 0:
            logger.warning(f"PDF '{path.name}' appears to be completely scanned or empty.")

        return pages
