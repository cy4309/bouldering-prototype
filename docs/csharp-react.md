# C# 對 React

對照這個專案裡已經寫過的 C#。右邊用 React 函式元件和一般 JavaScript。

一格只放一個寫法。多出來的一句話放在表下面。

## 檔案

| C# | React |
|---|---|
| `using UnityEngine;` | `import { ... } from "..."` |
| 檔名必須等於 class 名 | 檔名慣例上等於元件名 |
| 一個檔案一個 `public class` | 一個檔案一個元件 |

`using` 是把名稱引進來。`UnityEngine` 提供 `MonoBehaviour`、`Vector3`、`Debug`。`UnityEngine.InputSystem` 提供 `Keyboard`。

## 元件本身

| C# | React |
|---|---|
| `public class HoldContact : MonoBehaviour` | `function HoldContact() { ... }` |
| `: MonoBehaviour` | 沒有對應語法。這是在說「我是 Unity 元件」 |
| `void Awake()` | 元件函式本體裡、第一次 render 時做的事 |
| `void Update()` | `useFrame`，或一個持續執行的 effect |
| `void FixedUpdate()` | 沒有現成對應。物理步自己的迴圈 |

Unity 依方法名稱呼叫 `Awake`、`Update`。React 要自己把函式掛進 `useEffect` 或 `useFrame`。

## 資料

| C# | React |
|---|---|
| `public float speed = 1.5f;` | `function Climber({ speed = 1.5 })` |
| `Rigidbody body;` | `const bodyRef = useRef(null)` |
| `bool climbing;` | `const [climbing, setClimbing] = useState(false)` |
| `public bool IsHanging { get; private set; }` | state 只在這個元件裡用 `setIsHanging` 改 |
| `Hold currentHold;` | `const [currentHold, setCurrentHold] = useState(null)` |

`public` 欄位會出現在 Unity 的 Inspector，數值存在場景裡。改 Inspector 不會改 `.cs` 裡的那個預設值。

欄位沒寫 `public` 就是這個 class 私有。React 裡元件內部的 `useState`、`useRef` 也外面拿不到。

數字類型要寫出來。`float` 是小數，字面值加 `f`，例如 `1.5f`、`0f`。`bool` 是 `true` / `false`。

## 方法

| C# | React |
|---|---|
| `void HangOn(Hold hold)` | `function hangOn(hold) { ... }` |
| `void StartClimb(Hold hold)` | `function startClimb(hold) { ... }` |
| `Hold hold` | 參數。C# 要寫型別，JavaScript 不用 |
| `return;` | `return;` |

`void` 表示這個方法不回傳值。會回傳值時，把 `void` 換成那個型別，例如 `Hold FindHold()`。

## 判斷與空值

| C# | React |
|---|---|
| `if (hold == null)` | `if (hold == null)` |
| `if (climbing && hold != climbFromHold)` | 同樣寫法 |
| `if (!climbing && !IsHanging)` | 同樣寫法 |
| `hold == null` | `hold == null` 或 `!hold` |

`==` 在這裡是在問「是不是同一個物件」或「有沒有這個物件」。JavaScript 的 `===` 才是你習慣的嚴格相等；C# 的 `==` 對物件參考就是在比是不是同一個。

`null` 表示這個變數還沒指到任何物件。`GetComponent<Hold>()` 找不到時就回傳 `null`。

## 這個專案裡的幾行

| C# | 在 React 裡最接近的意思 |
|---|---|
| `body = GetComponent<Rigidbody>();` | 從同一個物件取出已經存在的剛體 |
| `new Vector3(0f, 2f, -6f)` | `{ x: 0, y: 2, z: -6 }` 或 `new THREE.Vector3(0, 2, -6)` |
| `Vector3.zero` | `{ x: 0, y: 0, z: 0 }` |
| `Vector3.forward` | `{ x: 0, y: 0, z: 1 }` |
| `"碰到 " + hold.name` | `` `碰到 ${hold.name}` `` |
| `Debug.Log(...)` | `console.log(...)`。只給開發時看 |
| `GameMessage.Show(message)` | `setMessage(message)`。畫面上那一行字 |
| `keyboard.spaceKey.isPressed` | 這一幀空白鍵正按著 |
| `keyboard.upArrowKey.isPressed` | 方向鍵。還有 `downArrowKey`、`leftArrowKey`、`rightArrowKey` |
| `body.useGravity = false` | 這個物件暫時不受重力 |
| `body.linearVelocity = Vector3.zero` | 速度歸零 |
| `hold != climbFromHold` | 碰到的是另一個岩點，不是出發的那個 |
| `body.position = startPosition` | 把剛體放到起點。改 `position` 才會跟著物理走 |
| `body.position.y < fallY` | 人的高度低過界線。每幀檢查，掉出平台就重生 |
| `bool leftGround` | `useState`。記住「已經離開地面」 |
| `sideways * climbSpeed` | 按 A 是 `-1`，按 D 是 `1`，再乘速度。上升時左右也用它 |
| `public bool isFinish` | `<Hold isFinish={true} />`。每個岩點可以各自勾 |
| `enum RouteColor { Blue, Orange }` | 一組固定名稱。Inspector 會變成下拉選單 |
| `bool hasRoute` | `useState(null)`。`false` 表示還沒選色 |

`GetComponent<Rigidbody>()` 的角括號是型別參數。意思是「跟我要一個 Rigidbody」。JavaScript 沒有這個寫法，因為變數不先宣告型別。

## 命名

| C# | React |
|---|---|
| `HoldContact`、`StartClimb`、`IsHanging` | 元件用大寫開頭。函式多半小寫開頭，例如 `startClimb` |
| `climbSpeed`、`currentHold` | 同樣是小寫開頭 |
| `WalkTowardWall.cs` | `WalkTowardWall.jsx` |

公開的方法、屬性、class 用大寫開頭。私有欄位用小寫開頭。這是這個專案正在用的寫法。
