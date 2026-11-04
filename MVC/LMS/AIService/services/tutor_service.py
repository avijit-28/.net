"""
Personalized Tutor & Code Assistant Service.
Handles programming explanations, debugging walkthroughs, and study plan creation.
"""
from typing import List, Dict, Any, Optional
from pydantic import BaseModel
from llm.provider_factory import llm_factory
from core.logger import get_masked_logger

logger = get_masked_logger(__name__)

TUTOR_SYSTEM_PROMPT = """You are an elite Lead Software Engineer and Academic CS Tutor at EduPlatform LMS.
Provide precise, pedagogical, step-by-step code walkthroughs, root-cause bug diagnoses, and production-ready corrections.
Always emphasize clean code, security best practices, and performance considerations."""

class StudyPlanItem(BaseModel):
    day: int
    topic: str
    suggested_lessons: List[str]
    estimated_minutes: int
    focus_objective: str

class StudyPlanResponse(BaseModel):
    course_title: str
    target_exam_or_goal: str
    total_days: int
    daily_schedule: List[StudyPlanItem]
    advice: str

class TutorService:
    def explain_code_or_error(
        self,
        code_snippet: str,
        error_message: Optional[str] = None,
        language: str = "csharp",
        question: Optional[str] = None
    ) -> Dict[str, Any]:
        logger.info(f"Tutor analyzing code ({language}) with error: {error_message is not None}")

        prompt = f"""LANGUAGE: {language}
USER QUESTION: {question or 'Explain this code and identify any potential bugs or performance bottlenecks.'}

CODE SNIPPET:
```{language}
{code_snippet}
```

ERROR / EXCEPTION TRACE (if any):
{error_message or 'No runtime error specified. Perform code review and walkthrough.'}

Please provide:
1. **Walkthrough**: What this code does line-by-line in plain, intuitive language.
2. **Analysis / Bug Detection**: Why the error occurs (or potential flaws/edge cases).
3. **Corrected Solution**: The fixed, production-ready code with comments.
4. **Best Practices**: 2-3 tips for {language} development to prevent this in the future."""

        res = llm_factory.generate(
            prompt=prompt,
            system_instruction=TUTOR_SYSTEM_PROMPT,
            temperature=0.2
        )

        return {
            "analysis": res.get("text", ""),
            "language": language,
            "has_error": bool(error_message),
            "tokens_used": res.get("total_tokens", 0),
            "provider_used": res.get("provider", "")
        }

    def generate_study_plan(
        self,
        course_title: str,
        completed_lessons: List[str],
        incomplete_lessons: List[str],
        available_days: int = 7,
        daily_hours: float = 1.5
    ) -> StudyPlanResponse:
        logger.info(f"Generating personalized {available_days}-day study plan for {course_title}")

        prompt = f"""You are an academic learning advisor for {course_title}.
A student has requested a {available_days}-day study plan ({daily_hours} hours/day).

STUDENT PROGRESS:
- Completed Lessons ({len(completed_lessons)}): {', '.join(completed_lessons) if completed_lessons else 'None yet'}
- Incomplete / Upcoming Lessons ({len(incomplete_lessons)}): {', '.join(incomplete_lessons) if incomplete_lessons else 'All completed, prepare for final revision'}

INSTRUCTIONS:
Create a realistic, step-by-step daily study schedule.
Prioritize uncompleted lessons, with built-in revision checkpoints and practice tests.
Return a warm, motivating tone."""

        res = llm_factory.generate(
            prompt=prompt,
            system_instruction="You are a compassionate, structured Academic Mentor at an enterprise university.",
            temperature=0.3
        )

        items = []
        step = max(1, len(incomplete_lessons) // max(1, available_days))
        for d in range(1, available_days + 1):
            sub_lessons = incomplete_lessons[(d-1)*step : d*step] if incomplete_lessons else ["Comprehensive Curriculum Review"]
            items.append(StudyPlanItem(
                day=d,
                topic=f"Day {d}: {' & '.join(sub_lessons[:2]) if sub_lessons else 'Curriculum Practice'}",
                suggested_lessons=sub_lessons,
                estimated_minutes=int(daily_hours * 60),
                focus_objective=f"Master key concepts in day {d} syllabus."
            ))

        return StudyPlanResponse(
            course_title=course_title,
            target_exam_or_goal="Course Mastery & Assessment Readiness",
            total_days=available_days,
            daily_schedule=items,
            advice=res.get("text", "")
        )

tutor_service = TutorService()
