using Microsoft.AspNetCore.Mvc;
using FitDevRecruit.Models;
using FitDevRecruit.Services;

namespace FitDevRecruit.Controllers
{
    public class RecruitController : Controller
    {
        private readonly QuestionService _questionService;
        private readonly TestService _testService;
        private readonly ReportService _reportService;

        public RecruitController(QuestionService questionService, TestService testService, ReportService reportService)
        {
            _questionService = questionService;
            _testService = testService;
            _reportService = reportService;
        }

        // 지원자 정보 입력 페이지
        public IActionResult Index()
        {
            return View();
        }

        // 지원자 정보 입력 처리
        [HttpPost]
        public IActionResult StartTest(Candidate candidate)
        {
            if (ModelState.IsValid)
            {
                candidate.Id = new Random().Next(1000, 9999);
                var questions = _questionService.GetRandomQuestions(5);
                
                // 세션에 지원자 정보 저장
                HttpContext.Session.SetString("CandidateName", candidate.Name);
                HttpContext.Session.SetString("CandidateEmail", candidate.Email);
                HttpContext.Session.SetString("ExperienceLevel", candidate.ExperienceLevel.ToString());
                HttpContext.Session.SetInt32("CandidateId", candidate.Id);
                
                return RedirectToAction("Test", new { candidateId = candidate.Id });
            }
            return View("Index", candidate);
        }

        // 문제 풀이 페이지
        public IActionResult Test(int candidateId)
        {
            var candidateName = HttpContext.Session.GetString("CandidateName");
            var candidateEmail = HttpContext.Session.GetString("CandidateEmail");
            var experienceLevel = HttpContext.Session.GetString("ExperienceLevel");
            
            ExperienceLevel level = ExperienceLevel.Junior;
            if (Enum.TryParse<ExperienceLevel>(experienceLevel, out var parsedLevel))
            {
                level = parsedLevel;
            }
            
            var questions = _questionService.GetQuestionsByLevelAndTeam(level, TeamType.Backend, 5);
            ViewBag.CandidateId = candidateId;
            ViewBag.ExperienceLevel = level;
            return View(questions);
        }

        // 답안 제출 처리
        [HttpPost]
        public IActionResult SubmitTest(int candidateId, Dictionary<int, string> answers)
        {
            var candidate = new Candidate
            {
                Id = candidateId,
                Name = HttpContext.Session.GetString("CandidateName") ?? "",
                Email = HttpContext.Session.GetString("CandidateEmail") ?? "",
                Answers = answers
            };

            // 채점 로직
            var questions = _questionService.GetAllQuestions();
            int technical = 0, personality = 0, problemSolving = 0;

            foreach (var answer in answers)
            {
                var question = questions.FirstOrDefault(q => q.Id == answer.Key);
                if (question != null)
                {
                    if (question.Type == QuestionType.MultipleChoice && int.TryParse(answer.Value, out int idx))
                    {
                        if (question.CorrectChoiceIndex == idx - 1) technical += 20;
                    }
                    else if (question.Type == QuestionType.Subjective)
                    {
                        if (!string.IsNullOrEmpty(question.Answer) && answer.Value?.Trim() == question.Answer.Trim())
                            problemSolving += 20;
                    }
                    else if (question.Type == QuestionType.Personality && int.TryParse(answer.Value, out int pScore))
                    {
                        personality += pScore;
                    }
                }
            }

            candidate.TechnicalScore = technical;
            candidate.PersonalityScore = personality;
            candidate.ProblemSolvingScore = problemSolving;

            var report = _reportService.GenerateReport(candidate);
            return View("Result", report);
        }

        // 결과 페이지
        public IActionResult Result()
        {
            return View();
        }
    }
} 