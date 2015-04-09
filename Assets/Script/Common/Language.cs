/**
 * 文件名称：Language.cs
 * 简    述：语言包;
 * 创建标识：wsy  2013/9/11
  */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Language
{
	#region ----------公用----------
    /// <summary>
    /// "确 定"
    /// </summary>
	public const string Confirm = "确 定";
    /// <summary>
    ///  "取 消"
    /// </summary>
	public const string Cancel = "取 消";
    /// <summary>
    /// "同 意"
    /// </summary>
	public const string Agree = "同 意";
    /// <summary>
    /// "拒 绝"
    /// </summary>
	public const string Refuse = "拒 绝";
    /// <summary>
    ///  "刷 新"
    /// </summary>
	public const string Refresh = "刷 新";
    /// <summary>
    ///  "提 示"
    /// </summary>
	public const string Prompt = "提 示";
    /// <summary>
    ///  "是"
    /// </summary>
	public const string Yes = "是";
    /// <summary>
    ///  "否"
    /// </summary>
	public const string No = "否";
    /// <summary>
    ///  "秒"
    /// </summary>
    public const string Second = "秒";
    /// <summary>
    ///  "无"
    /// </summary>
    public const string None = "无";
    /// <summary>
    ///  "胜利"
    /// </summary>
    public const string Win = "胜利";
    /// <summary>
    ///  "失败"
    /// </summary>
    public const string Fail = "失败";
    /// <summary>
    ///  "成功"
    /// </summary>
    public const string Suceess = "成功";
    /// <summary>
    ///  "购买"
    /// </summary>
    public const string Buy = "购买";
    /// <summary>
    ///  "价格："
    /// </summary>
    public const string Price = "价格：";
    /// <summary>
    ///  "原价："
    /// </summary>
    public const string OriginaPrice = "价格：";
    /// <summary>
    ///  "得分"
    /// </summary>
    public const string Score = "得分";
    /// <summary>
    /// {"战士","射手","法师"}
    /// </summary>
    public static string[] JobSign=new string[]{"战士","射手","法师"};
    /// <summary>
    /// { "蜀国", "吴国", "魏国", "群雄" }
    /// </summary>
    public static string[] CountrySign = new string[] { "蜀国", "吴国", "魏国", "群雄" };
    /// <summary>
    ///  { "female","male" }
    /// </summary>
    public static string[] SexSign = new string[] { "female","male" };

    /// <summary>
    ///  "请选择操作对象"
    /// </summary>
    public const string PleaseChooseTarget = "请选择操作对象";
    /// <summary>
    ///  "请选择操作对象"
    /// </summary>
    public const string IsMoneySign = "是否使用一千元宝补签";
    /// <summary>
    ///  "请选择操作对象"
    /// </summary>
    public const string MoneySign = "补签";
	#endregion 
	
	#region ---------wsy--------------
    /// <summary>
    ///  "【领导力：{0}/{1}】"
    /// </summary>
    public const string LeaderShip = "【领导力：{0}/{1}】";
    /// <summary>
    ///  "已达等级上限"
    /// </summary>
    public const string ReachMaxLevel = "已达等级上限";
    /// <summary>
    ///  "超过角色等级"
    /// </summary>
    public const string MoreRoleLevel = "卡牌等级大于角色等级，无法强化";
    /// <summary>
    ///  "是否进行背包格扩充?"
    /// </summary>
    public const string IsBuyBagGrid = "确定进行背包格扩充?";
    /// <summary>
    ///  "必杀技："
    /// </summary>
    public const string KillSkill = "必杀技：";
    /// <summary>
    ///  "统帅技："
    /// </summary>
    public const string CaptainSkill = "统帅技：";
    /// <summary>
    ///  "等级不足"
    /// </summary>
    public const string NotOpenLevel = "等级不足";
    /// <summary>
    ///  "选择武将"
    /// </summary>
    public const string SelectHero = "选择武将";
    /// <summary>
    ///  "援助武将"
    /// </summary>
    public const string AidHero = "援助武将";
    /// <summary>
    ///  "卡牌满级后才能进化"
    /// </summary>
    public const string LevelNotFit = "卡牌满级后才能进化";
    /// <summary>
    ///  "请选择助阵玩家"
    /// </summary>
    public const string PleaseSelectAidPlayer = "请选择助阵玩家";
    /// <summary>
    ///  "请选择要出售的卡牌"
    /// </summary>
    public const string PleaseSelectCardToSell = "请选择要出售的卡牌";
    /// <summary>    ///  "请输入账号"
    /// </summary>
    public const string InputAccount = "请输入账号";
    /// <summary>
    ///  "请输入名称"
    /// </summary>
    public const string InputName = "请输入名称";
    /// <summary>
    ///  "确定退出游戏？"
    /// </summary>
    public const string IsExit = "确定退出游戏？";
    /// <summary>
    ///  "该功能暂未开放"
    /// </summary>
    public const string NotOpen = "该功能暂未开启";
    /// <summary>
    ///  "正在努力加载场景文件..."
    /// </summary>
    public const string BeLoadingSceneFile = "正在努力加载场景文件...";
    /// <summary>
    ///  "正在检测资源更新... "
    /// </summary>
    public const string BeingCheckResUpdate = "正在检测资源更新... ";
    /// <summary>
    ///  "元宝不足 "
    /// </summary>
    public const string NoEnoughGold = "元宝不足";
    /// <summary>
    ///  "金钱不足 "
    /// </summary>
    public const string NoEnoughMoney = "金币不足";
    /// <summary>
    ///  "碎片不足 "
    /// </summary>
    public const string NoEnoughDebris = "碎片不足";
    /// <summary>
    ///  "背包格不足 "
    /// </summary>
    public const string NoEnoughBag = "背包格不足";
    /// <summary>
    ///  "友情点不足 "
    /// </summary>
    public const string NoEnoughFriendPoint = "友情点不足";

    #region ---背包Bag---
    /// <summary>
    ///  "请选择{0}的主卡"
    /// </summary>
    public const string SelectMainCard = "请选择{0}的主卡";
    /// <summary>
    ///  "请选择{0}的材料卡"
    /// </summary>
    public const string SelectMaterialCard = "请选择{0}的素材卡";
    /// <summary>
    ///  "缺少{0}的材料卡"
    /// </summary>
    public const string NoMaterialCard = "缺少{0}的素材卡";
    /// <summary>
    ///  "强化"
    /// </summary>
    public const string Upgrade = "强化";
    public static string[] UpgradeMode = new string[] { "普通强化", "特级强化", "神级强化" };
    public static string[] UpgradeModeDetail = new string[] { "小机率出现1.5-2倍经验暴击", "消耗金币翻倍，较大机率出现1.5-2倍经验暴击", "消耗20元宝，必定出现1.5-3倍经验暴击" };

    /// <summary>
    /// "进化"
    /// </summary>
    public const string Evolve = "进化";
    /// <summary>
    /// "技能升级"
    /// </summary>
    public const string SkillUpgrade = "技能升级";
    #endregion ---商店---

  
     #region ---主角---
    public static string[] CharactarTitles = new string[] { "组卡", "布阵", "背包","天赋" };
    #endregion ---主角---

    #region ---社交---
    /// <summary>
    ///  "确定要将玩家【{0}】从好友列表中删除？"
    /// </summary>
    public const string IsDeleteFriend = "确定要将玩家【{0}】从好友列表中删除？";
    /// <summary>
    ///  "确定要将玩家【{0}】加为好友？"
    /// </summary>
    public const string IsAddFriend = "确定要将玩家【{0}】加为好友？";
    #endregion ---社交---

    #region ---商店---
    public const string Store = "商店";
    public const string VipRight = "VIP特权";
   
    /// <summary>
    /// { "友情点抽取", "元宝抽取【随机】", "元宝抽取【魏国】", "元宝抽取【蜀国】", "元宝抽取【吴国】", "元宝抽取【群雄】" }
    /// </summary>
    public static string[] CardBuyType = new string[] { "友情点抽取", "元宝抽取【随机】", "元宝抽取【魏国】", "元宝抽取【蜀国】", "元宝抽取【吴国】", "元宝抽取【群雄】" };
    #endregion ---商店---

    #region ---系统---
    public const string System = "系统";
    public static string[] SystemTitles = new string[] { "？月游戏公告", "图鉴", "帮助", "欢迎联系我们", "设置" };
    public const string GameNotice = "游戏公告";   
    #endregion---系统---

    #region ---卡牌资料---
    public const string HeroExp = "英雄经验：";
    public const string SkillExp = "技能经验：";
    public const string Job = "职业：";
    public const string Skill = "特性技：";
    public const string Skill2 = "统帅技：";
    public const string Attack = "攻击：";
    public const string Crit = "暴击：";
    public const string AttackSpeed = "攻速：";
    public const string ReliveTime = "复活时间：";
    public const string CritRate = "暴击率：";     
    public const string HP = "生命：";
    public const string Dodge = "闪避：";
    #endregion---卡牌资料---

    #region ---公会---
    /// <summary>
    /// "职位："
    /// </summary>
    public const string GuildJob = "职位：";
    /// <summary>
    ///  "等级："
    /// </summary>
    public const string Level = "等级：";
    /// <summary>
    ///  "贡献点："
    /// </summary>
    public const string GuildContribute = "贡献点：";
    /// <summary>
    ///  "贡献点："
    /// </summary>
    public const string GuildDeclaration = "公会宣言：";
    /// <summary>
    /// { "公会列表", "我的公会", "公会成员","公会科技","入会审核"}
    /// </summary>
    public static string[] GuildTitles = new string[] { "公会列表", "我的公会", "公会成员","公会科技","入会审核"};
    /// <summary>
    ///  "确定要离开公会？"
    /// </summary>
    public const string IsLeaveGuild = "确定要离开公会？";
    /// <summary>
    ///  "确定要解散公会？"
    /// </summary>
    public const string IsDiemissGuild = "确定要解散公会？";
    /// <summary>
    ///  "确定要解散或者离开公会？"
    /// </summary>
    public const string IsDiemissorleftGuild = "请你选择解散或者离开帮会？";
    /// <summary>
    ///  "解散公会"
    /// </summary>
    public const string DiemissGuild = "解散帮会";
    /// <summary>
    ///  "离开公会"
    /// </summary>
    public const string LeftGuild = "离开帮会";
    /// <summary>
    ///  "设置描述公会"
    /// </summary>
    public const string SetGuildDes = "设置描述";
    /// <summary>
    ///  "设置公会图标"
    /// </summary>
    public const string SetGuildIcon = "设置图标";
    /// <summary>
    ///  "确定要解散或者离开公会？"
    /// </summary>
    public const string ChoiceSettingGuild = "请你选择设置描述或者图标？";
    /// <summary>
    ///  "确定要加入【{0}】公会"
    /// </summary>
    public const string IsAddGuild = "确定要加入【{0}】公会？";
    /// <summary>
    ///  "确定要将【{0}】踢出公会"
    /// </summary>
    public const string IsTickGuildMember = "确定要将【{0}】踢出公会？";
    /// <summary>
    ///  "确定要将会长权限移交给【{0}】？"
    /// </summary>
    public const string IsAssignGuildLeader = "确定要将会长权限移交给【{0}】？";
    /// <summary>
    ///  "确定同意【{0}】加入公会？"
    /// </summary>
    public const string IsAgreeJoinGuild = "确定同意【{0}】加入公会？";
    /// <summary>
    ///  "确定拒绝【{0}】加入公会？"
    /// </summary>
    public const string IsRefuseJoinGuild = "确定拒绝【{0}】加入公会？";
    #endregion---公会---
    #endregion---------wsy--------------

}
