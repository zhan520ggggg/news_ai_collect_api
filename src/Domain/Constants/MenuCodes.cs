namespace Domain.Constants;

/// <summary>
/// 菜单编码常量
/// </summary>
public static class MenuCodes
{
    // 模块
    public const string Collection = "collection";
    public const string Hotspot = "hotspot";
    public const string DataAnalysis = "data_analysis";
    public const string System = "system";

    // 热点采集模块
    public const string CollectionConfig = "collection_config";
    public const string CollectionMonitor = "collection_monitor";
    public const string CollectionData = "collection_data";

    // 热点管理模块
    public const string HotspotList = "hotspot_list";
    public const string HotspotEdit = "hotspot_edit";
    public const string HotspotCategoryTag = "hotspot_category_tag";
    public const string HotspotReview = "hotspot_review";
    public const string HotspotPublish = "hotspot_publish";
    public const string HotspotArchive = "hotspot_archive";

    // 数据分析展示模块
    public const string DataDashboard = "data_dashboard";
    public const string DataTrend = "data_trend";
    public const string DataCollection = "data_collection";
    public const string DataOperations = "data_operations";

    // 系统管理模块
    public const string SystemMenus = "system_menus";
    public const string SystemRole = "system_role";
    public const string SystemConfig = "system_config";
    public const string SystemBackup = "system_backup";
    public const string SystemNotification = "system_notification";
}
