"""
EduPlatform AIService - Centralized LLM Provider Factory with Automatic Failover
Routes queries between Google Gemini (primary) and Groq (secondary) seamlessly.
"""

from typing import Dict, Any, Optional
from config import settings
from core.logger import logger
from core.exceptions import LLMQuotaExceededError
from llm.gemini_provider import GeminiProvider
from llm.groq_provider import GroqProvider

class LLMProviderFactory:
    def __init__(self):
        self.gemini = GeminiProvider()
        self.groq = GroqProvider()
        self.default_provider = (settings.DEFAULT_LLM_PROVIDER or "gemini").lower()

    def generate(
        self,
        prompt: str,
        system_instruction: str,
        temperature: float = None,
        max_tokens: int = None
    ) -> Dict[str, Any]:
        """
        Attempts generation with primary provider. If rate-limited or quota exceeded,
        automatically falls back to the alternative provider.
        """
        primary = self.gemini if self.default_provider == "gemini" else self.groq
        secondary = self.groq if self.default_provider == "gemini" else self.gemini
        primary_name = "gemini" if self.default_provider == "gemini" else "groq"
        secondary_name = "groq" if self.default_provider == "gemini" else "gemini"

        # 1. Try Primary
        if primary.is_configured():
            try:
                res = primary.generate(prompt, system_instruction, temperature, max_tokens)
                res["failover_occurred"] = False
                return res
            except LLMQuotaExceededError:
                logger.warning(f"Primary provider '{primary_name}' quota exhausted. Engaging automatic failover to '{secondary_name}'...")
            except Exception as e:
                logger.warning(f"Primary provider '{primary_name}' failed ({e}). Attempting failover to '{secondary_name}'...")

        # 2. Try Secondary Failover
        if secondary.is_configured():
            try:
                res = secondary.generate(prompt, system_instruction, temperature, max_tokens)
                res["failover_occurred"] = True
                logger.info(f"Failover successful using secondary provider: {secondary_name}")
                return res
            except Exception as e:
                logger.error(f"Secondary provider '{secondary_name}' also failed: {e}")

        # 3. Deterministic Local Semantic Response Fallback
        logger.warning("All LLM providers unavailable or unconfigured. Triggering local deterministic fallback.")
        return {
            "text": "[EduBot Academic Advisor Notice]: Both external LLM providers reached temporary free-tier rate limits. Please review the retrieved course syllabus sections provided below.",
            "model": "local-fallback",
            "provider": "local",
            "prompt_tokens": len(prompt.split()),
            "completion_tokens": 25,
            "total_tokens": len(prompt.split()) + 25,
            "failover_occurred": True
        }

llm_factory = LLMProviderFactory()
