"""
EduPlatform AIService - DOCX Document Extractor
Extracts structured paragraphs, headings, and table cells from Microsoft Word documents.
"""

from pathlib import Path
from typing import List, Dict, Any
import docx
from extractors.text_cleaner import clean_text, is_text_meaningful
from core.logger import logger
from core.exceptions import DocumentExtractionError

class DocxExtractor:
    @staticmethod
    def extract(file_path: str) -> List[Dict[str, Any]]:
        path = Path(file_path)
        if not path.exists():
            raise DocumentExtractionError(f"File not found: {file_path}")

        try:
            doc = docx.Document(file_path)
            logger.info(f"Extracting DOCX '{path.name}'...")

            sections: List[Dict[str, Any]] = []
            current_section_title = "Document Overview"
            current_text_buffer: List[str] = []
            current_page = 1
            word_count_in_page = 0

            # Process paragraphs
            for para in doc.paragraphs:
                text = para.text.strip()
                if not text:
                    continue

                # Check if paragraph is a heading
                if para.style and para.style.name and para.style.name.startswith("Heading"):
                    current_section_title = text

                current_text_buffer.append(text)
                word_count_in_page += len(text.split())

                # Approximate a page boundary every ~450 words
                if word_count_in_page >= 450:
                    combined = clean_text("\n".join(current_text_buffer))
                    if is_text_meaningful(combined):
                        sections.append({
                            "page_number": current_page,
                            "section_title": current_section_title,
                            "text": combined
                        })
                        current_page += 1
                        current_text_buffer = []
                        word_count_in_page = 0

            # Process tables
            for table in doc.tables:
                table_rows = []
                for row in table.rows:
                    row_cells = [clean_text(cell.text) for cell in row.cells]
                    table_rows.append(" | ".join(row_cells))
                if table_rows:
                    current_text_buffer.append("\nTable:\n" + "\n".join(table_rows))

            # Flush remaining buffer
            if current_text_buffer:
                combined = clean_text("\n".join(current_text_buffer))
                if is_text_meaningful(combined):
                    sections.append({
                        "page_number": current_page,
                        "section_title": current_section_title,
                        "text": combined
                    })

            if not sections:
                raise DocumentExtractionError(f"DOCX document '{path.name}' contains no readable text.")

            return sections

        except Exception as ex:
            logger.error(f"Failed to extract DOCX '{path.name}': {ex}")
            raise DocumentExtractionError(f"Error extracting DOCX: {str(ex)}")
