# MTM 인게임 유닛 설계 과제

## 개요

챔피언, 미니언, 타워의 공통 동작을 Unit 추상 클래스로 묶고, 세 유닛의 핵심 차이인
**죽었을 때 어떻게 되는가**를 각 클래스의 `Die()` / `DummyDestroy()` override 여부와
내용으로 표현했습니다.

`DummyMonoBehaviour`가 제공된 의도(Unity 엔진 없이도 검증 가능한 순수 로직)에 맞춰
구체적인 구현(타겟 탐색 알고리즘, 공격력 계산식, 리스폰 타이머 등)은 과제 지시에 따라 스텁으로 남겨두고 설계 구조와 책임 분리에 집중했습니다.

---

## 1. 파일 구성

| 파일 | 설명 |
|---|---|
| `DummyMonoBehaviour.cs` | 제공된 원본 파일 |
| `HPBar.cs` | 제공된 원본 파일 (RefreshHP 파라미터를 float으로 조정) |
| `BattleManager.cs` | 제공된 원본 파일 |
| `Unit.cs` | 챔피언/미니언/타워의 공통 베이스 클래스 |
| `Champion.cs` | 죽으면 일정 시간 후 부활 |
| `Minion.cs` | 죽으면 소멸, 생성 시 게임 시간에 비례한 공격력 세팅 |
| `Tower.cs` | 죽으면 영구 파괴 |
| `HpController.cs` | (선택 과제) Unit과 HPBar를 연결하는 클래스 |

---

## 2. 필수 과제 - Unit 설계

### 2-1. 공통 베이스: `Unit`

- `TakeDamage()`로 체력이 0 이하가 되면 `Die()`를 호출하는 흐름은 세 유닛이 공유합니다.
- `Die()`는 **"게임 로직상 죽음"**을 의미하고, `DummyDestroy()`는 **"인스턴스 생명주기의
  끝"**을 의미하도록 역할을 분리했습니다. 이 구분 덕분에 챔피언처럼 죽어도 인스턴스가
  계속 살아있어야 하는 경우와, 미니언/타워처럼 죽음이 곧 소멸로 이어지는 경우를
  하나의 베이스 클래스로 자연스럽게 표현할 수 있습니다.
- `OnHpChanged`, `OnDeath` 이벤트를 노출해서, 이 유닛의 상태 변화에 관심 있는 외부
  객체(HPBar, 이펙트 시스템, 사운드 등)가 무엇이든 구독할 수 있게 했습니다. `Unit`은
  누가 구독하는지 전혀 알지 못합니다.
- `FindTarget()`은 abstract로 남겨서, "누구를 공격할지 정하는 규칙"이 유닛마다
  다르다는 설계 의도만 표현하고 실제 탐색 알고리즘은 생략했습니다.

### 2-2. Champion - 부활

```csharp
protected override void Die()
{
    base.Die();
    ScheduleRespawn(respawnDelay);
    // DummyDestroy()를 호출하지 않음
}
```

`DummyDestroy()`를 호출하지 않는 것 자체가 "인스턴스가 계속 살아있다"는 것을
의미합니다. 일정 시간 뒤 `Respawn()`이 호출되어 체력 등 상태만 초기화되고, 인스턴스
자체는 재사용됩니다. (실제 타이머 구현은 생략)

### 2-3. Minion - 소멸 + 게임 시간 비례 공격력

```csharp
public override void DummyAwake()
{
    base.DummyAwake();
    attackDamage = GetProperAttackDamageBy(BattleManager.CurrentGameSeconds);
}

protected override void Die()
{
    base.Die();
    DummyDestroy();
}
```

생성 시점(`DummyAwake`)에 `BattleManager.CurrentGameSeconds`를 기반으로 공격력을
세팅합니다. 죽을 때는 `DummyDestroy()`까지 호출해서 인스턴스가 재사용되지 않고
소멸됨을 명시했습니다.

### 2-4. Tower - 영구 파괴

```csharp
protected override void Die()
{
    base.Die();
    DummyDestroy();
}

public override void DummyDestroy()
{
    base.DummyDestroy();
    // Tower 소멸 시 골드/경험치 지급 등 부가 로직 (구체 구현 생략)
}
```

Minion과 마찬가지로 `Die()` → `DummyDestroy()` 흐름을 갖지만, `DummyDestroy()`를
override해서 소멸 시점의 부가 로직(보상 지급 등)을 위한 확장 지점을 남겨뒀습니다.
부활 로직이 코드 어디에도 없다는 사실 자체로 "영구 파괴"를 표현했습니다.

### 2-5. 세 유닛 비교

| 유닛 | `Die()`에서 하는 일 | `DummyDestroy()` 호출 | 의미 |
|---|---|---|---|
| Champion | `ScheduleRespawn()` | 호출 안 함 | 인스턴스 생존, 나중에 부활 |
| Minion | 없음 | 호출함 | 인스턴스 소멸, 재사용 안 됨 |
| Tower | 없음 | 호출함 (override로 보상 로직 확장) | 인스턴스 영구 파괴 |

---

## 3. 선택 과제 - HP 표시 구조 (Unit ↔ HPBar 디커플링)

### 설계 목표

`Unit`은 `HPBar`라는 타입을 전혀 몰라야 하고, `HPBar`도 `Unit`을 몰라야 합니다.
이를 위해 **Observer 패턴 + 중간 연결 클래스**로 구조를 나눴습니다.

```
[Unit]  --(OnHpChanged 이벤트)-->  [HpController]  --(RefreshHP 호출)-->  [HPBar]
   ↑ HPBar를 모름                        ↑ 둘 다 아는 유일한 클래스           ↑ Unit을 모름
```

### 책임 분리

| 요소 | 책임 |
|---|---|
| `Unit.OnHpChanged` | "HP가 바뀌었다"는 사실만 알림. 구독자가 무엇인지 신경 쓰지 않음 |
| `HPBar.RefreshHP(float)` | 전달받은 값을 표시. 누가 호출했는지 신경 쓰지 않음 |
| `HpController` | Unit과 HPBar를 둘 다 참조하는 유일한 클래스. 이 클래스가 곧 "연결"이라는 책임을 전담 |

이렇게 분리한 이유는, 나중에 HPBar가 다른 형태의 UI(텍스트, 미니맵 표시 등)로 바뀌거나
여러 UI가 동시에 하나의 유닛을 구독해야 하는 상황이 오더라도 `Unit` 클래스는 전혀
수정할 필요가 없게 하기 위함입니다. 반대로 Unit의 전투 로직이 바뀌어도 HPBar 쪽
코드는 영향받지 않습니다.

### `Unbind()`에 대해

```csharp
//이 이벤트 구독만 제거하는 안전장치용 함수 현재는 사용X
public void Unbind()
{
    _unit.OnHpChanged -= HandleHpChanged;
}
```

`Unit.DummyDestroy()`에서 `OnHpChanged = null`로 이벤트를 일괄 정리하긴 하지만
이는 "Unit이 소멸할 때"만 발생합니다. Champion처럼 죽어도 소멸하지 않는 유닛의 경우
혹은 Unit은 살아있는데 그 Unit을 보여주던 HPBar/HpController 쪽만 먼저 정리해야 하는
상황(UI 재구성 등)에는 대응할 수 없습니다. `Unbind()`는 이런 경우를 위해 자신의
구독만 정확히 제거할 수 있도록 남겨둔 안전장치이며 현재 과제 범위에서는 실제 호출 시점(어떤 이벤트에서 호출되어야 하는지)까지는 구현하지 않았습니다.

### `HpController` 생성 위치
`HpController`의 생성 및 연결은 `Unit`과 `HPBar`가 모두 준비된 시점(유닛 스폰 로직)에서 이 과제의 스코프 밖인 별도의 조립 지점이 담당하는 것으로 가정합니다. `Unit`이나 `HPBar` 내부에서 스스로 `HpController`를 생성하지 않는 것은 그 순간 Unit이 HPBar를 간접적으로나마 참조하게 되어 디커플링 원칙이 깨지기 때문입니다.

---

## 4. 검증

`mcs`(Mono C# Compiler)로 전체 파일을 컴파일하여 문법 오류가 없음을 확인했고,
간단한 실행 테스트로 다음을 확인했습니다.

- Champion은 `Die()` 이후에도 인스턴스가 유지됨 (부활 대기 상태)
- Minion, Tower는 `Die()` 직후 `DummyDestroy()`까지 호출되어 소멸/파괴 처리가 완료됨
- `HpController`를 통해 `Unit.TakeDamage()` 호출 시 `HPBar.RefreshHP()`가 정상적으로
  트리거됨 (Unit, HPBar 상호 간 직접 참조 없이 동작)
