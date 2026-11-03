### 1. 프로젝트 개요
- 메뉴 알림(Red Dot) 시스템 샘플 프로젝트
- 게임 UI의 컨텐츠(우편함·도감·던전 등)별 알림 표시를 모델 상태 변화에 따라 하위에서 상위로 자동 집계하는 구조를 구현한 Unity 샘플

### 2. 핵심 기능
- 계층형 알림 집계: 슬롯(개별 항목) → 컨텐츠 아이콘 → 확장 버튼(마스터)
- 컨텐츠 3종
   - 우편함(Mail): 우편 수신 시 알림
   - 도감(Codex): 탭별 항목 추가 시 알림
   - 던전(Dungeon): 유저 레벨에 따라 해금 시 알림
- 슬롯 상태: Locked / New / Confirmed. New일 때만 알림 노출
- 치트 윈도우: 에디터 툴(CheatWindow)로 우편·도감 추가, 유저 레벨 변경을 테스트

### 3. 아키텍처
- View / Model 분리: ModelPanel<TModel>이 Bind(model)로 모델 연결. 패널은 표시만, 데이터와 로직은 Model이 담당
- 알림 흐름
   - Model이 INotifySource.Changed(NotifyKey, bool) 이벤트를 발행
   - ModelPanel.Bind 시 NotifyManager.Register로 자동 구독 (OnDestroy에서 해제)
   - NotifyManager가 컨텐츠별 비트 플래그(1 << flag)로 상태 관리
   - 변경 시 OnContentsIconNotify(아이콘)와 OnMasterNotify(확장 버튼) 이벤트 발행
   - SampleWorld와 ContentsPanel이 이벤트를 받아 UI 갱신
- NotifyKey.Of<TFlag>(contents, enumFlag): 컨텐츠 내부 enum을 플래그로 쓸 수 있어, 새 알림 종류를 코드 한 줄로 추가 가능
- 범용 StateSlot: 버튼·이미지·텍스트·알림 아이콘을 묶은 재사용 슬롯
- SampleWorld: 모델 소유, 패널 바인딩, 패널 열기/닫기를 조율하는 진입점

### 4. 영상
https://github.com/user-attachments/assets/af2910cb-54d4-4a5f-82a3-4cb35b728aed

