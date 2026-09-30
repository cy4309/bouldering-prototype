# 抱石教程筆記

Unity 6000.6.2f1 · URP · `Assets/Scenes/SampleScene.unity`

Three.js 是命令式寫法。R3F 是 React Three Fiber。兩邊都是 Y 朝上。Unity 相機看向 **+Z**，Three.js / R3F 相機看向 **-Z**。

## 進度

現在有藍、橘兩條交錯的路線。按住空白鍵只會往上，A、D 或左右方向鍵自己決定左右。跳得不夠高、太早橫移，會碰到另一色並失敗。這個交錯路線尚未在 Play 確認。

- 完成：`Floor`、`Wall`、`Climber`、藍橘各三顆岩點、`WalkTowardWall`、`FollowClimber`、`Hold`、`HoldContact`、`Ground`
- 還沒做：手能搆多遠、手點抓取、Prefab、攀爬動畫

Play 之後先點 **Game** 視窗。

| 按鍵 | 效果 |
|---|---|
| W 或 ↑、S 或 ↓、A 或 ←、D 或 → | 在地面移動。↑ 走向岩壁 |
| A D 或 ← →（停住時） | 沿著岩壁左右挪，高度不變。碰到旁邊的岩點就改停在那裡 |
| 空白鍵 | 按住時往上爬，鬆開就開始掉。不會自動吸向下一顆 |
| S 或 ↓（停住時） | 鬆手落下 |
| 上升中鬆開空白鍵 | 從空中落下 |
| 離開地面後再碰到地面 | 回到起點 `(0, 1, -2)`，速度歸零 |
| 掉到 `fallY`（-1）以下 | 視為失敗，回到起點，顏色清空 |
| 停在同色且勾了 Is Finish 的岩點 | 畫面上方出現 `路線完成`，只印一次 |
| 碰到另一種顏色 | 畫面上方出現 `路線失敗`，回到起點，顏色清空 |

## 這個專案

| 物件 | 是什麼 |
|---|---|
| `Main Camera` | `FollowClimber`，在人後上方 `(0, 2, -6)`，看向人 |
| `Floor` | 深灰綠材質 `Materials/Floor`。Plane，Scale `(2, 1, 2)` |
| `Wall` | 暖灰岩色材質 `Materials/Wall`。Cube，`(0, 3, 3)`，Scale `(12, 6, 0.4)` |
| `Climber` | 灰色材質 `Materials/Climber`。Capsule。Rigidbody、`WalkTowardWall`、`HoldContact` |
| `HoldBlue1` | 藍，起點。`(-4.08, 1.2, 2.65)` |
| `HoldOrange2` | 橘。`(-0.83, 2.19, 2.65)` |
| `HoldBlue2` | 藍。`(-1.22, 2.81, 2.65)` |
| `HoldBlue3` | 藍，終點。`(-5.04, 5.1, 2.65)` |
| `HoldOrange1` | 橘，起點。`(2.2, 1.2, 2.65)` |
| `HoldOrange3` | 橘，終點。`(1.7, 4.2, 2.65)` |

岩點不要拖進 `Wall`。`Wall` 的 Scale 會把子物件一起拉扁。

| 腳本 | 掛在 | 只做這件事 |
|---|---|---|
| `WalkTowardWall` | `Climber` | 水平移動 |
| `HoldContact` | `Climber` | 辨認岩點、上升、停住、鬆手、回到起點 |
| `Ground` | `Floor` | 標記「這是地面」 |
| `FollowClimber` | `Main Camera` | 相機跟隨 |
| `Hold` | 六顆岩點 | 標記岩點，並記下藍或橘 |

攀爬用兩個狀態記住：正在往上，或停在岩點上。

| 狀態 | 條件 | 結果 |
|---|---|---|
| 往上 | 按住空白鍵 | 只加向上的速度。A、D 同時改左右。鬆開就掉 |
| 停住 | 往上時碰到同色的另一顆 | `useGravity = false`。沒按 A、D 時速度是 0 |
| 左右挪 | 停住時按 A 或 D | 只改 `linearVelocity.x`。碰到另一色則失敗 |
| 選擇 | 第一次按空白鍵離開一顆岩點 | `hasRoute = true`，記住那顆的 `routeColor` |
| 換手 | 停住時碰到同色的另一顆 | 停住目標改成新的岩點 |
| 完成 | 停住同色、且勾了 `isFinish` | Console 印一次 `路線完成` |
| 失敗 | 碰到另一種顏色 | Console 印 `路線失敗`，回到起點，`hasRoute = false` |
| 落下 | 停住時按 S，或往上時鬆開空白鍵 | `useGravity = true` |
| 回到起點 | 曾經離開地面，再次撞到帶 `Ground` 的物件 | 回到 `(0, 1, -2)`，速度歸零，已選顏色清掉 |

停住或往上時，`WalkTowardWall` 看到 `IsHanging` 或 `IsClimbing`，這一幀不改水平速度。再往上之前要先鬆開空白鍵。

往上時不再自動飛向下一顆同色岩點。按住空白鍵維持向上的速度，A、D 改左右。鬆開空白鍵後重力把人拉下來，碰到哪顆就算哪顆。另一色是失敗。

## 對照

每一列只放名稱。需要多記的一句話寫在該表下面。

### 專案

| Unity | Three.js | R3F |
|---|---|---|
| `Assets` | `src` | `src` |
| `Packages/manifest.json` | `package.json` | `package.json` |
| `Library` | `node_modules` | `node_modules` |

### 場景

| Unity | Three.js | R3F |
|---|---|---|
| Scene 檔 | `Scene` | `<Canvas>` 裡的內容 |
| GameObject | `Object3D` | `<group>` |
| Component | 物件上的屬性 | 元件或 ref |
| `Transform` | `.position` `.rotation` `.scale` | 同名 props |
| 拖進父子層級 | `parent.add(child)` | JSX 包在裡面 |
| `MeshFilter` + `MeshRenderer` | `Mesh` | `<mesh>` |
| Cube / Plane / Capsule | `BoxGeometry` / `PlaneGeometry` / `CapsuleGeometry` | `<boxGeometry>` / `<planeGeometry>` / `<capsuleGeometry>` |
| Material | `MeshStandardMaterial` | `<meshStandardMaterial>` |
| Camera | `PerspectiveCamera` | `<PerspectiveCamera>` |
| Directional Light | `DirectionalLight` | `<directionalLight>` |
| Game 視窗 | `renderer.render(scene, camera)` | Canvas 畫出來的那一塊 |
| Play | `requestAnimationFrame` | `<Canvas>` 掛載後 |

Scene 視窗是編輯時自由飛行的視角。Game 視窗才是場景裡那台相機。

### 腳本

| Unity | Three.js | R3F |
|---|---|---|
| `.cs`，檔名等於 class 名 | `.js` 模組 | 元件檔 |
| `: MonoBehaviour` | 普通 class | `function HoldContact()` |
| `Awake` | `constructor` | 元件第一次執行 |
| `Update` | 動畫迴圈的一幀 | `useFrame` |
| `FixedUpdate` | `world.step` | 物理套件自己的 step |
| `LateUpdate` | 同一幀裡較晚的那段 | 寫在角色更新之後的 `useFrame` |
| `Time.deltaTime` | clock delta | `useFrame` 的 `delta` |
| `public` 欄位 | 建構參數 | props |
| 拖一個 `Transform` 進欄位 | 變數存另一個物件 | prop 傳 ref |
| `GetComponent<T>()` | 讀同一物件上的屬性 | 同一個元件裡的 ref |
| 空的 `Hold` | `userData.isHold` | `userData` 或空元件 |
| `Debug.Log` | `console.log` | `console.log` |

`Awake`、`Update`、`FixedUpdate`、`LateUpdate`、`OnCollisionEnter`、`OnCollisionStay`、`OnCollisionExit` 都是 Unity 依名稱呼叫。不用自己叫。

| 方法 | 時機 |
|---|---|
| `Awake` | 建立時一次 |
| `Update` | 每幀 |
| `FixedUpdate` | 每個物理步 |
| `LateUpdate` | 這幀的 `Update` 之後 |
| `OnCollisionEnter` | 剛撞上 |
| `OnCollisionStay` | 還撞在一起 |
| `OnCollisionExit` | 剛分開 |

### 移動與物理

| Unity | Three.js | R3F |
|---|---|---|
| `transform.position` | `mesh.position` | `ref.current.position` |
| Rigidbody | Rapier / Cannon body | `<RigidBody>` |
| Collider | collider | `<CuboidCollider>`、`<CapsuleCollider>` |
| `linearVelocity` | `setLinvel` | `setLinvel` |
| `useGravity` | gravity scale | `gravityScale` |
| Freeze Rotation | lock rotations | `lockRotations` |
| `OnCollisionEnter` | collision event | `onCollisionEnter` |
| `Vector3.forward` `(0,0,1)` | 相機前方是 `(0,0,-1)` | 同 Three.js |
| `LookAt` | `lookAt` | `useFrame` 裡 `lookAt` |

直接改 `transform.position` 是瞬移，碰撞體不會擋。這個專案的走路改的是 `linearVelocity`。

### 鍵盤

| Unity | Three.js | R3F |
|---|---|---|
| `Keyboard.current` | `keydown` / `keyup` | `KeyboardControls` |
| `key.isPressed` | 自己用 Set 記著按住 | `get().forward` 這類狀態 |

### 畫面上的字

`Canvas` 是蓋在遊戲上的一層，像網頁裡 `position: fixed` 的區塊。`Text` 是那一行字。`Debug.Log` 只進 Console，玩家看不到。`GameMessage.Show` 把同一句畫在畫面上方。

### 放到網頁

Unity 的 Web 輸出是一個資料夾，裡面有 `index.html`。對照 Three.js，就是把場景編成瀏覽器能跑的檔案，而不是在 React 裡直接寫 `<Canvas>`。

**Switch Platform** 只改「下次 Build 要產出哪種檔」。編輯器仍在 Windows 上跑，Play 也還能測。要回 Windows 匯出時，再把平台切回 **Windows 64-bit**。

切到 Web 之後，在 **Build Profiles** 按 **Build And Run**。輸出資料夾放在專案外面。第一次建置會比較久。

預設的 `index.html` 帶有載入條和全螢幕按鈕，那是網頁模板，可以換成自己的頁面，只留遊戲畫布。遊戲一開始的 Unity 標誌是 Splash Screen，免費版拿不掉。

整包放上網站的靜態目錄，現有頁面用 `<iframe src="/bouldering/index.html">` 嵌進去。`Build` 裡的 `.loader.js`、`.data`、`.framework.js`、`.wasm` 要跟 `index.html` 留在同一個相對路徑。壓縮是 Brotli 時，伺服器要對 `.br` 送出 `Content-Encoding: br`。做不到就把 **Compression Format** 改成 **Disabled** 再打包。

手機現在不能玩。操作只讀 `Keyboard.current`，觸控不會送出 WASD 或空白鍵。Unity 的 Web 也是給桌面瀏覽器。

**Quality** 裡平台打勾只代表「這檔可以用」。綠色的 **Default** 才是開起來用的那檔。Web 的預設仍是 `Mobile`：畫面縮成 0.8 倍，陰影圖 1024、只有一層，所以陰影有顆粒。PC 那檔是 2048、四層。把 Web 的綠色預設改到 `PC` 後要重新 Build。

### Git

版控的是這個專案資料夾。`.gitignore` 排除 Unity 會重產的 `Library`、`Temp`、`Logs`、`UserSettings`，以及 `Build`、`*.apk`。要進版控的是 `Assets`（含 `.meta`）、`Packages`、`ProjectSettings`、`docs`。
