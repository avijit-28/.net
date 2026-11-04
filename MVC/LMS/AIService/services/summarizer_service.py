"""
Summarizer Service for RAG AI Learning Assistant.
Produces executive summaries, key concept breakdowns, and high-yield revision notes.
"""
from typing import List, Dict, Any, Optional
from pydantic import BaseModel
from llm.provider_factory import llm_factory
from core.logger import get_masked_logger

logger = get_masked_logger(__name__)

SUMMARIZER_SYSTEM_PROMPT = """You are a senior academic research mentor and curriculum summarizer at EduPlatform LMS.
Your mission is to produce crystal-clear, structured educational summaries, concept definitions,
and revision bullet points grounded in the provided course material."""

class SummaryResult(BaseModel):
    title: str
    summary_type: str
    summary_text: str
    key_points: List[str]
    suggested_revision_questions: List[str]
    tokens_used: int = 0
    provider_used: str = ""

class SummarizerService:
    def summarize_content(
        self,
        title: str,
        content_text: str,
        summary_type: str = "revision_notes"
    ) -> SummaryResult:
        logger.info(f"Summarizing '{title}' with mode {summary_type}")

        prompt = f"""Generate a structured {summary_type.replace('_', ' ').upper()} for the following educational material.

TITLE: {title}

CONTENT:
{content_text[:8000]}

STRUCTURE YOUR RESPONSE AS FOLLOWS:
1. **Executive Overview**: 2-3 sentence core summary.
2. **Key Concepts & Technical Definitions**: 3-5 core definitions.
3. **High-Yield Revision Checkpoints**: 5-7 bullet points for rapid exam recall.
4. **Self-Check Review Questions**: 3 reflective questions for testing student comprehension."""

        res = llm_factory.generate(
            prompt=prompt,
            system_instruction=SUMMARIZER_SYSTEM_PROMPT,
            temperature=0.2
        )
        response_text = res.get("text", "")

        lines = response_text.splitlines()
        key_points = [line.strip().lstrip("*-•123456789. ") for line in lines if line.strip().startswith(("-", "*", "•"))][:7]

        return SummaryResult(
            title=title,
            summary_type=summary_type,
            summary_text=response_text,
            key_points=key_points,
            suggested_revision_questions=[
                f"How would you explain the core concepts of {title} in your own words?",
                f"What are the main real-world software applications of {title}?",
                f"What are the most common pitfalls to avoid when working with {title}?"
            ],
            tokens_used=res.get("total_tokens", 0),
            provider_used=res.get("provider", "")
        )

summarizer = SummarizerService()
