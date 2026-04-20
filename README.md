# Clean Architecture API

基于 .NET 8 的多层架构项目，包含完整的依赖注入、仓储模式、统一响应、全局异常处理、Swagger 规范化、AutoMapper 映射、FluentValidation 数据校验。

---

## 项目结构总览

```
CleanArchitecture/
├── src/
│   ├── Common/                          # 公共层（零依赖）
│   │   └── Models/
│   │       ├── ApiResponse.cs           # 统一响应模型
│   │       ├── PagedResponse.cs         # 分页模型
│   │       └── Exceptions.cs            # 业务异常体系
│   │
│   ├── Domain/                          # 领域层（核心业务规则）
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs            # 实体基类（Id, 时间戳, 软删除）
│   │   │   ├── User.cs                  # 用户实体
│   │   │   ├── Role.cs                  # 角色实体
│   │   │   └── Product.cs               # 产品实体
│   │   ├── Interfaces/
│   │   │   ├── IRepository.cs           # 泛型仓储接口
│   │   │   └── IUserRepository.cs       # 用户仓储接口
│   │   └── Constants/
│   │       └── RoleNames.cs             # 角色名称常量
│   │
│   ├── Application/                     # 应用层（业务逻辑）
│   │   ├── DTOs/
│   │   │   ├── UserDto.cs               # 用户 DTO（Create/Update/Response）
│   │   │   ├── ProductDto.cs            # 产品 DTO
│   │   │   └── AuthDto.cs               # 认证 DTO（Login/Register/LoginResponse）
│   │   ├── Interfaces/
│   │   │   ├── IUserService.cs          # 用户服务接口
│   │   │   ├── IProductService.cs       # 产品服务接口
│   │   │   └── IJwtTokenProvider.cs     # JWT Token 提供者接口
│   │   ├── Services/
│   │   │   ├── UserService.cs           # 用户服务实现
│   │   │   └── ProductService.cs        # 产品服务实现
│   │   ├── Validators/
│   │   │   ├── UserValidator.cs         # FluentValidation 用户校验
│   │   │   └── ProductValidator.cs      # FluentValidation 产品校验
│   │   ├── Extensions/
│   │   │   └── ApplicationServiceExtension.cs  # 应用层自动注册扩展
│   │   └── Mappings/
│   │       └── MappingProfile.cs        # AutoMapper 配置
│   │
│   ├── Infrastructure/                  # 基础设施层（数据访问/EF Core 配置/外部服务调用/文件存储、消息队列等 专门干脏活累活）
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs  # EF Core DbContext
│   │   │   └── DbSeeder.cs              # 数据库种子数据（角色 + 默认管理员）
│   │   ├── Repositories/
│   │   │   ├── Repository.cs            # 泛型仓储实现
│   │   │   └── UserRepository.cs        # 用户仓储实现
│   │   └── Extensions/
│   │       └── InfrastructureServiceExtension.cs  # DI 扩展
│   │
│   └── WebApi/                          # 表现层（API 入口）
│       ├── Controllers/
│       │   ├── AuthController.cs        # 认证接口（登录/注册/当前用户）
│       │   ├── UsersController.cs       # 用户 CRUD 接口
│       │   └── ProductsController.cs    # 产品 CRUD 接口
│       ├── Middleware/
│       │   └── GlobalExceptionMiddleware.cs  # 全局异常中间件
│       ├── Filters/
│       │   └── ApiResponseFilter.cs     # 统一响应过滤器
│       ├── Config/
│       │   ├── JwtConfig.cs             # JWT 配置类
│       │   ├── JwtTokenProvider.cs      # JWT Token 生成实现
│       │   └── SwaggerConfig.cs         # Swagger 规范化配置
│       ├── Program.cs                   # 应用入口
│       └── appsettings.json             # 配置文件
```

额外知识
Domain（接口）= 菜谱（系统核心都在菜谱）
Infrastructure（实现）= 厨房

这样设计的真正价值
① 解耦：换数据库不用动业务
    今天用 EF + MySQL明天想换 Dapper + PostgreSQL只需要改 Infrastructure，Domain 完全不动。
    不仅Domain不用动，WebApi不用动，Common不用动，Application也不用动！！
② 方便单元测试
    以在测试里直接做个假的 Repository：public class FakeUserRepository : IUserRepository { ... }
    领域逻辑完全不受影响。
③ 符合依赖方向规则
    依赖方向永远是：外部层 → 内部层
    Infrastructure → Domain
    Domain 绝不向外依赖。
④ 业务代码更干净
    领域层只看业务，不看 SQL、不看连接字符串、不看 DbContext。

---

## 形象的比喻

## 1业务依赖
WebApi（前台） ← Common（点菜台）
  ↓ 依赖
Application（服务员） ← Common（餐具）
  ↓ 依赖
Domain（菜谱） ← Common（餐具） 
  ↑ 实现
Infrastructure（厨房） ← Common（餐具）
## 2第三方依赖
Application（临时工，电工，水工） ← Common（餐具）
  ↑ 实现
Infrastructure（劳务中心） ← Common（餐具）

服务员（Application）不能指挥前台（WebApi）  ：实现业务接口
菜谱（Domain）不依赖任何人  ：Domain 层应该"零外部依赖"，也不定义第三方技术，专注业务领域，就像菜谱不需要关心水管坏了，验钞机，点菜机坏了 谁来修
厨房（Infrastructure）按菜谱（Domain） ：做菜，实现所有外部接口（数据库，第三方）
餐具（Common）大家都可以用

## 架构特性

| 特性 | 实现方式 |
|---|---|
| **多层架构** | Controller → Service → Repository → DbContext |
| **依赖注入** | 按层注册，接口→实现分离；应用层支持约定扫描自动注册 |
| **仓储模式** | `IRepository<T>` 泛型接口 + 软删除支持 |
| **统一响应** | 所有返回 `ApiResponse<T>` 格式 `{code, message, data, timestamp}` |
| **全局异常** | `GlobalExceptionMiddleware` 按异常类型返回对应 HTTP 状态码 |
| **跨域配置** | CORS 策略通过 `AllowedOrigins` 配置化 |
| **Swagger** | 启用 XML 注释 + Bearer 认证 + 首页展示 |
| **AutoMapper** | DTO ↔ Entity 自动映射 |
| **数据校验** | FluentValidation 统一校验，DTO 为纯 POCO |
| **软删除** | 基类 `IsDeleted` 字段 + EF Core QueryFilter |
| **分页** | `PagedRequest` / `PagedResponse<T>` 通用分页模型 |
| **JWT 认证** | Bearer Token + 角色 Claim + 自动过期验证 |
| **角色体系** | 5 个预置角色，多对多关联，启动时自动种子数据 |
| **密码安全** | BCrypt 加密（work factor 12） |

---

## 统一响应格式示例

```json
// 成功
{"code":0,"message":"success","data":{...},"timestamp":"2026-04-15T..."}

// 校验失败 (422)
{"code":422,"message":"用户名不能为空; 密码不能为空","data":null,"timestamp":"..."}

// 资源不存在 (404)
{"code":404,"message":"User (Id=xxx) 不存在","data":null,"timestamp":"..."}

// 冲突 (409)
{"code":409,"message":"邮箱 'xxx' 已存在","data":null,"timestamp":"..."}

// 未认证 (401)
{"code":401,"message":"用户名或密码错误","data":null,"timestamp":"..."}
```

---

## 认证接口

| 接口 | 方法 | 说明 | 需要认证 |
|---|---|---|---|
| `/api/auth/login` | POST | 用户登录 | 否 |
| `/api/auth/register` | POST | 用户注册 | 否 |
| `/api/auth/me` | GET | 当前用户信息 | 是 |

### 登录示例

```bash
# 登录（默认管理员: admin / Admin@123）
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userName":"admin","password":"Admin@123"}'

# 响应
{"code":0,"message":"success","data":{
  "token":"eyJhbGci...",
  "expiresAt":"2026-04-15T...",
  "user":{"id":"...","userName":"admin","email":"admin@system.local"}
}}

# 携带 Token 访问
curl http://localhost:5000/api/auth/me \
  -H "Authorization: Bearer <token>"

# 注册新用户
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"userName":"newuser","email":"new@test.com","password":"123456"}'
```

### 角色列表

| 角色名称 | 显示名称 | 说明 |
|---|---|---|
| `SuperAdmin` | 超级管理员 | 拥有系统所有权限 |
| `ContentReviewer` | 内容审核员 | 负责内容审核与管理 |
| `CollectionAdmin` | 采集管理员 | 负责数据采集与管理 |
| `DataAnalyst` | 数据分析员 | 负责数据分析与报表 |
| `OperationsAdmin` | 运营管理员 | 负责系统运营与配置 |

---

## 启动方式

```bash
cd src/WebApi
dotnet run --urls "http://localhost:5000"
```

- **Swagger UI**: [http://localhost:5000](http://localhost:5000)
- **健康检查**: [http://localhost:5000/health](http://localhost:5000/health)
---


## 终止
taskkill /F /IM dotnet.exe
或者精确到进程 ID
taskkill /F /PID <进程号>

## 新增服务（无需修改 Program.cs）

只需两步：

```csharp
// 1. 定义接口（Application.Interfaces 命名空间）
namespace Application.Interfaces;
public interface IOrderService { }

// 2. 实现类（Application.Services 命名空间）
namespace Application.Services;
public class OrderService : IOrderService { }
```

`ApplicationServiceExtension.AddApplication()` 会自动扫描注册。
