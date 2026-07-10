# C# Console Game Framework

## 실행 방법

```bash
dotnet run
```

Visual Studio에서는 `ConsoleGameFramework.csproj` 파일을 열고 실행

## 폴더 구조

```text
ConsoleGameFramework/
├─ Program.cs
├─ Common/ # Utility, Constants 등 전역 상수 및 유틸 함수
├─ Core/      # 게임 루프, 화면 전환, 공용 상태 (GameManager, GameContext, IScene, SceneBase, SceneKey)
├─ Data/      # 게임 진행에 필요한 읽기 전용 데이터 
├─ Items/     # 게임 내 아이템 관련 함수, 클래스
├─ Model/    # 게임 내에서 필요한 객체 클래스(Character, Player,Enemy ... )
├─ Skills/      # 게임 내 스킬에 관련된 함수 및 데이터(Skill, SkillManager, Buff, Debuff ...)
├─ UI/        # 콘솔 출력, 입력, 더블 버퍼링, 표/박스/맵 렌더링 (ConsoleUI)
└─ Scenes/    # 화면 예시 (TitleScene, SampleScene)
```

### 프로그램 기본 실행 흐름

```
Program.cs
  └─ GameManager.Instance.Run()
        └─ while (게임이 실행 중이라면)
              1. 현재 Scene.Render(context)   // 화면 내용을 버퍼에 그림
              2. ConsoleUI.Present()          // 버퍼 내용을 콘솔에 한 번에 출력
              3. 현재 Scene.HandleInput(context) // 입력을 받아 다음 행동 결정
```

### 기본 흐름 

게임 시작 -> 마을 정비 [스킬 강화, 아이템 구매] -> 던전 진입 -> 적 조우[최소 1회] -> 보스 조우
