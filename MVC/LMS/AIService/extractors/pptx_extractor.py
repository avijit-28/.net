"""
EduPlatform AIService - PPTX Presentation Extractor
Extracts slide titles and bullet points from PowerPoint presentations.
"""

from pathlib import Path
from typing import List, Dict, Any
from pptx import Presentation
from extractors.text_cleaner import clean_text, is_text_meaningful
from core.logger import logger
from core.exceptions import DocumentExtractionError

class PptxExtractor:
    @staticmethod
    def extract(file_path: str) -> List[Dict[str, Any]]:
        path = Path(file_path)
        if not path.exists():
            raise DocumentExtractionError(f"File not found: {file_path}")

        try:
            prs = Presentation(file_path)
            slides_data: List[Dict[str, Any]] = []
            total_slides = len(prs.slides)
            logger.info(f"Extracting PPTX '{path.name}' ({total_slides} slides)...")

            for slide_idx, slide in enumerate(prs.slides):
                slide_num = slide_idx + 1
                slide_title = f"Slide {slide_num}"
                slide_texts: List[str] = []

                for shape in slide.shapes:
                    if shape.has_text_frame:
                        for paragraph in shape.text_frame.paragraphs:
                            t = paragraph.text.strip()
                            if t:
                                slide_texts.append(t)

                if slide.shapes.title and slide.shapes.title.text.strip():
                    slide_title = slide.shapes.title.text.strip()

                combined = clean_text("\n".join(slide_texts))
                if is_text_meaningful(combined, min_chars=10):
                    slides_data.append({
                        "page_number": slide_num,
                        "section_title": slide_title,
                        "text": combined
                    })

            if not slides_data:
                raise DocumentExtractionError(f"PPTX presentation '{path.name}' contains no readable text content.")

            return slides_data

        except Exception as ex:
            logger.error(f"Failed to extract PPTX '{path.name}': {ex}")
            raise DocumentExtractionError(f"Error extracting PPTX: {str(ex)}")
