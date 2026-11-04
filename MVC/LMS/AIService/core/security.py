"""
EduPlatform AIService - Security & Authorization Middleware
Validates service-to-service internal keys and enforces course-level authorization.
"""

from typing import List, Optional
from fastapi import Header, HTTPException, status
from config import settings
from core.logger import logger

class UserSecurityContext:
    def __init__(self, user_id: str, role: str, authorized_course_ids: List[int]):
        self.user_id = user_id
        self.role = role
        self.authorized_course_ids = authorized_course_ids

    def is_course_authorized(self, course_id: Optional[int]) -> bool:
        if course_id is None:
            return True
        # Admins have global curriculum access
        if self.role.lower() == "admin":
            return True
        return course_id in self.authorized_course_ids

async def verify_service_auth(
    x_internal_api_key: Optional[str] = Header(None, alias="X-Internal-API-Key"),
    x_user_id: Optional[str] = Header(None, alias="X-User-Id"),
    x_user_role: Optional[str] = Header("Student", alias="X-User-Role"),
    x_authorized_courses: Optional[str] = Header("", alias="X-Authorized-Courses"),
) -> UserSecurityContext:
    """
    Validates that the incoming request originates from the verified ASP.NET MVC backend
    and parses the authenticated user's authorization boundaries.
    """
    expected_key = settings.AISERVICE_INTERNAL_KEY
    if expected_key and x_internal_api_key != expected_key:
        logger.warning("Unauthorized access attempt: Invalid X-Internal-API-Key.")
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Unauthorized: Invalid internal service credentials."
        )

    # Parse authorized course IDs (e.g. "1,4,7")
    course_ids = []
    if x_authorized_courses:
        for c in x_authorized_courses.split(","):
            c_clean = c.strip()
            if c_clean.isdigit():
                course_ids.append(int(c_clean))

    user_id = x_user_id or "anonymous"
    role = x_user_role or "Student"
    return UserSecurityContext(user_id=user_id, role=role, authorized_course_ids=course_ids)
