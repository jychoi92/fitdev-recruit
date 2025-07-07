# FitDev - 개발자 채용 온라인 평가 시스템

## 프로젝트 구조

```
FitDev/
├── .git/                    # Git 버전 관리
└── FitDevRecruit/          # 메인 프로젝트 폴더
    ├── Controllers/         # MVC 컨트롤러
    ├── Models/             # 데이터 모델
    ├── Services/           # 비즈니스 로직 서비스
    ├── Views/              # Razor 뷰
    ├── wwwroot/            # 정적 파일 (CSS, JS, 이미지)
    ├── Properties/         # 프로젝트 속성
    ├── questions.json      # 문제 데이터 (102문제)
    ├── FitDevRecruit.csproj # 프로젝트 파일
    ├── Program.cs          # 애플리케이션 진입점
    ├── appsettings.json    # 애플리케이션 설정
    ├── Dockerfile          # Docker 컨테이너 설정
    └── .gitignore          # Git 무시 파일
```

## 주요 기능

### 1. 팀별 맞춤형 문제 출제
- **백엔드**: C#, .NET, 데이터베이스, 아키텍처
- **프론트엔드**: HTML/CSS/JS, React, UI/UX
- **데이터**: SQL, Python, 데이터 분석, 머신러닝
- **인프라**: Linux, 네트워크, 클라우드, 보안

### 2. 경력별 차별화된 평가
- **신입**: 기술 기초 + 코딩테스트(1-2문제) + 인성
- **경력**: 기술 심화 + 설계 + 리더십 + 문제해결력

### 3. 자동 채점 시스템
- 객관식: 정답 5점, 오답 0점
- 주관식/코딩: 완전 일치 5점, 부분 일치 2점
- 인성: 선택지별 점수 또는 1-5점

### 4. 종합 리포트 생성
- 기술, 인성, 문제해결력 영역별 점수
- 팀/경력별 맞춤형 평가 기준
- 개선점 및 강점 분석

## 실행 방법

### 1. 개발 환경 실행
```bash
cd FitDevRecruit
dotnet restore
dotnet run
```

### 2. Docker 실행
```bash
cd FitDevRecruit
docker build -t fitdev-recruit .
docker run -p 5000:80 fitdev-recruit
```

### 3. 빌드
```bash
cd FitDevRecruit
dotnet build
```

## 문제 데이터

- **총 102문제** (JSON 형식)
- 팀별/경력별/카테고리별 분류
- 객관식, 주관식, 인성 문제 포함
- 코딩테스트 문제 포함

## 기술 스택

- **Backend**: ASP.NET Core (.NET 9.0)
- **Frontend**: Razor Pages, Bootstrap, jQuery
- **Data**: JSON 파일 기반
- **Deployment**: Docker 지원

## 라이선스

이 프로젝트는 개발자 채용 평가 목적으로 제작되었습니다. 