"""
EduPlatform AIService - End-to-End RAG QA Pipeline
Performs query embedding, filtered vector retrieval, grounded prompt construction,
LLM invocation with failover, and citation synthesis.
"""

import time
from typing import List, Dict, Any, Optional
from config import settings
from core.logger import logger
from embeddings.local_embedder import embedder
from vectorstore.qdrant_manager import qdrant
from llm.provider_factory import llm_factory
from rag.prompt_templates import build_rag_prompt

class RagPipeline:
    def answer_query(
        self,
        query: str,
        course_id: Optional[int] = None,
        mode: str = "CourseAssistant",
        chat_history: Optional[List[Dict[str, str]]] = None,
        top_k: int = None
    ) -> Dict[str, Any]:
        start_time = time.time()
        k = top_k or settings.TOP_K

        # 1. Embed query locally
        query_vector = embedder.embed_text(query)

        # 2. Retrieve relevant chunks from Qdrant with course isolation
        hits = qdrant.search_authorized_course(
            query_vector=query_vector,
            course_id=course_id,
            top_k=k,
            threshold=settings.SIMILARITY_THRESHOLD
        )

        # 3. Format Context Blocks & Source Citations
        context_parts: List[str] = []
        sources: List[Dict[str, Any]] = []

        for idx, hit in enumerate(hits, 1):
            text = hit.get("text", "")
            page_num = hit.get("page_number")
            section = hit.get("section_title") or f"Section {idx}"
            doc_id = hit.get("document_id")
            score = hit.get("score", 0.0)

            ref_tag = f"[Source {idx} - Page {page_num}]" if page_num else f"[Source {idx}]"
            context_parts.append(f"{ref_tag} ({section}):\n{text}")

            sources.append({
                "source_index": idx,
                "document_id": doc_id,
                "chunk_id": hit.get("chunk_id"),
                "source_title": section,
                "page_number": page_num,
                "relevance_score": round(score, 4),
                "excerpt": text[:220] + ("..." if len(text) > 220 else "")
            })

        context_blocks = "\n\n".join(context_parts)

        # 4. Build prompt
        system_inst, user_prompt = build_rag_prompt(
            query=query,
            context_blocks=context_blocks,
            mode=mode,
            chat_history=chat_history
        )

        # 5. Generate Answer with Automatic Gemini -> Groq Failover
        llm_response = llm_factory.generate(
            prompt=user_prompt,
            system_instruction=system_inst,
            temperature=settings.TEMPERATURE,
            max_tokens=settings.MAX_OUTPUT_TOKENS
        )

        duration_ms = int((time.time() - start_time) * 1000)

        return {
            "answer": llm_response.get("text", ""),
            "sources": sources,
            "model_used": llm_response.get("model", ""),
            "provider_used": llm_response.get("provider", ""),
            "failover_occurred": llm_response.get("failover_occurred", False),
            "prompt_tokens": llm_response.get("prompt_tokens", 0),
            "completion_tokens": llm_response.get("completion_tokens", 0),
            "total_tokens": llm_response.get("total_tokens", 0),
            "duration_ms": duration_ms,
            "mode": mode,
            "course_id": course_id
        }

rag_pipeline = RagPipeline()
