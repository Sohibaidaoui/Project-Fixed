using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AotForms
{
    internal static class Config
    {
        internal static bool VisualsEnabled = false;

        internal static bool speed = false;
        internal static bool rgb = false;
        internal static float iconsize = 0.5f;
        public static Vector4 ICONCOLOR = new Vector4(1f, 1f, 1f, 1f);
        internal static bool espweapon = false;  
        internal static bool ESPFillBox = false;  
        internal static bool EspUp = false;  
        internal static bool FixEsp = false;  
        internal static bool minimap = false;  
        internal static bool ESPWeapon = false;  
        internal static float AimBotSmooth = 0f;
        internal static AimBotType AimBotType;
        internal static bool FirstOfAllTurnThisOn = false;
        internal static bool enableAimBot = false;
        internal static bool AimLock = false;
        internal static bool TriggerBot = false;
        internal static bool DrawClosest = false;
        internal static bool AutoLoadConfig = false;
        internal static bool AimBotRage = false;
        internal static bool SilentAim = false;
        internal static bool AimbotRubix = false;
        internal static bool AimbotVisible = false;
        internal static bool kcsafeSilentAim = false;
        internal static bool kcbrutasilnetaim = false;
        internal static bool RapidFire = false;
        internal static bool FASTFIREONN = false;
        internal static float RapidFireSpeed = 0.5f;
        internal static bool FastReload = false;
        internal static bool UpPlayer = false;
        internal static bool tele = false;
        internal static bool ShakeKill = false;
        internal static bool flyhack = false;
        internal static bool UnlimitedAmmo = false;
        internal static bool teliportwall = false;
        internal static bool alignment = true;


        internal static Keys AimbotKey = Keys.LButton; 

        internal static int AimBotMaxDistance = 200;

        internal static bool IgnoreKnocked = false;
        internal static bool UpdateEntities = false;
        internal static bool NoRecoil = false;
        internal static bool NoCache = false;
        internal static int AimbotSmoothness = 0;
        internal static int AimBotFov = 100000;
        internal static bool speedint = false;

        internal static TargetingMode TargetingMode  = TargetingMode.ClosestToCrosshair;

        internal static bool ESPLine = false;
        internal static bool EspShowDragonLogo = true;
        internal static Color ESPLineColor = Color.White;
        internal static Color ESPFillBoxColor = Color.White;
        internal static float AimFov = 200f;
        internal static int AimFovCircle = 500;
        internal static float Esprender = 80f;
        /// <summary>Max distance (m) for ESP match timer enemy detection (reference: espran).</summary>
        internal static float espran = 200f;
        internal static bool StreamMode = false;
        /// <summary>Fullscreen slow-falling rotating dragon icons (decorative UI particles).</summary>
        internal static bool UiDragonParticleRain = true;

        internal static Color NameCheat = Color.Red;
        internal static bool ESPBox = false;
        internal static float speedVal = 5.0f;
        internal static bool Speed = false;
        internal static bool speedext = false;
        internal static bool wallhack = false;

        internal static Color ESPBoxColor = Color.White;
        internal static bool teliport = false;
        internal static bool Kcspawnkill = false;
        internal static float SilentFOV = 12000000000000000000f;
        internal static bool ClimbUpEnabled = false;
        internal static bool ClimbUpV2Enabled = false;
        internal static float ClimbUpV2Value = 12f;
        internal static bool DownPlayer = false;
        internal static bool FlyHack50x = false;
        internal static bool FlyHack50xx = false;
        internal static bool FlyHack80x = false;
        internal static bool FlyHack80xx = false;
        internal static bool FLYHACKINTTTT = false;
        internal static bool FLYHACKINTTT = false;
        internal static bool FLYHACKX400 = false;
        internal static bool FLYHACKX40 = false;
        internal static bool TeleportAnywhere = false;
        internal static bool directteleport = false;
        internal static bool EnemyPullEnabled = false;
        internal static bool EnemyPullEnabledv2 = false;
        internal static bool forceaim = false;
        internal static bool undergroundkill = false;
        internal static bool MAGNETPULL = false;
        internal static bool FlyHack = false;
        internal static bool spinbot = false;
        internal static float SpinBotSpeed = 4.2f;
        internal static bool highjumppp = false;
        internal static bool cameraleftt = false;
        internal static bool visionhackkk = false;
        internal static bool flyhacks = false;
        internal static bool teleportmap = false;
        internal static bool brsutfiree = false;

        internal static bool enableAndroidCheats = false;
        internal static bool androidSilentKill = false;
        internal static bool androidAutoFire = false;
        internal static bool androidAimkillSend = false;
        internal static bool androidAutoKill = false;
        internal static bool androidSpeedRun = false;
        internal static bool androidFlyHack = false;
        internal static float androidFov = 5000f;
        internal static bool IgnoreBots = false;
        internal static bool ShowTeammates = false;
        internal static bool MuteSound = false;

        internal static int EnemyPullTickMs = 6;
        internal static float EnemyPullMaxDistance = 150f;
        internal static float recoilcontrol = 10f;
        internal static float UpPlayerSpeed = 20f;
        internal static float speedTP = 10f;
        internal static bool ESPInformation = false;
        internal static bool ESPScreenPanel = false; // Fixed on-screen enemy list panel
        internal static bool espdistance = false;
        internal static bool ESPName = false;
        internal static Color ESPNameColor = Color.White;
        internal static bool ESPHealth = false;
        internal static bool ESPHealthText = false;
        internal static Color ESPHealthColor = Color.Green;
        internal static bool ESPSkeleton = false;  
        public static bool EspScreenSmooth = true;
        internal static Color ESPSkeletonColor = Color.White;
        internal static bool ESPRGB = false;
        internal static bool AimTrackLine = false;
        internal static Color AimTrackLineColor = Color.Red;
        internal static bool HackerTag = false;
        internal static Color HackerTagColor = Color.Red;
        internal static bool EspTimer = false;
        internal static bool ESPEditor = false;
        internal static bool Watermark = true;
        internal static bool EnemyTimerActive = false;
        internal static System.DateTime EnemyTimerStart = System.DateTime.MinValue;
        internal static System.DateTime LastEnemySeen = System.DateTime.MinValue;
        /// <summary>ESP valid-match countdown length in seconds (Silent Max = 180).</summary>
        internal const int EspMatchTimerDurationSec = 180;
        internal static int EnemyTimerDuration = EspMatchTimerDurationSec;
        internal static int ResetAfterNoEnemy = 40;
        public static string InvalidText = "Game invalid! Minimum time not completed.";
        public static string ValidText = "Game completed in: {0}";
        internal static bool FOVEnabled = false;

        public static bool SilentKillEnabled = false;    // Enable/disable the main silent kill feature
        public static bool SilentKillDragEnabled = false; // Enable/disable the optional drag teleport


        
        internal static bool AimBot = false;
        
        internal static bool AimSilent = false;
        internal static bool FastFire = false;
        internal static float AntiAimSpeed = 360f;

        internal static bool SilentAim360 = false;
    
        // SR Divine Aimbot
        internal static bool SR_DIVINE_AimbotEnabled = false;
        internal static bool SR_DIVINE_IgnoreKnocked = false;
        internal static int SR_DIVINE_MaxDistance = 250;
        internal static int SR_DIVINE_FOVRadius = 200;
        internal static int SR_DIVINE_SmoothnessFactor = 1;
        internal static bool Teleport = false;
        internal static bool silentnormal = false;
        internal static bool SilentAimvisible = false;
        internal static bool aimlegit = false;
  
    
        internal static bool aimIsVisible = true;

        internal static bool Aimfovc = false;
        internal static bool AimfovcRGB = false;
        internal static string AimBotMode = "Hex";

     
        internal static Keys AimSilentKey = Keys.LButton;

        // Enemy Pull Settings
  
        internal static int EnemyPullMaxDistanceNormal = 150;
       
        internal static float EnemyPullStrength = 0.05f;
        internal static bool pullbabu = false;
        internal static bool AntiAimEnabled = false;


        internal static bool TeleportMarkEnabled = false;
        internal static bool TeleportV2 = false;

        internal static int Aimfov = 400;
        internal static float AimSmoothness = 0f;
        internal static float AimKillFOV = 30f;
        internal static Color Aimfovcolor = Color.Red;



        // ==========================================
        // 👁️ ESP: LINES SETTINGS
        // ==========================================
        internal static string ESPLineMode = "None";
        internal static bool ESPLineRGB = false;
        internal static bool ESPLineGlow = false;
        internal static bool ESPLineGlowRGB = false;
        internal static int EspLineThickNess = 1;
        internal static bool EspBottom = false;
        internal static bool EspDown = false;

        // ==========================================
        // 📦 ESP: BOX & FILL SETTINGS
        // ==========================================
        internal static bool ESPBox2 = false;
        internal static bool ESPBoxRGB = false;
        internal static bool BoxGlow = false;

        internal static bool ESPFillBoxRGB = false;
        internal static bool ESPFillBoxGlow = false;
        internal static bool ESPFillBoxGlowRGB = false;
        internal static bool ESPFilllBoxGlowRGB = false;
        internal static Color ESPFillBoxGlowColor = Color.White;

        internal static bool SILENTMAX = false;

        internal static bool ESPCorner = false;
        internal static bool ESPCorneredBox = false;
        internal static bool ESPCorneredBoxRGB = false;
        internal static bool ESPCorneredBoxFlow = false;
        internal static bool ESPCorneredBoxFlowRGB = false;
        internal static bool ESPCornerColor = false;
        internal static Color ESPCorneredBoxColor = Color.White;

        internal static bool ESPFullBoxGlow = false;
        internal static bool ESPFullBoxGlowRGB = false;
        internal static Color ESPFullBoxColor = Color.White;

        // ==========================================
        // 👤 ESP: PLAYER & INFO SETTINGS
        // ==========================================
        internal static bool ESPNameRGB = false;


      
        internal static Color ESPHeath = Color.White;



        internal static bool ESPDistance = false;
        internal static bool ESPWeaponIcon = false;
        internal static bool ESPInfo = false;
        internal static bool ESPclosest = false;
        internal static Color ESPclosestColor = Color.White;
        internal static bool esptotalplyer = false;

        // ==========================================
        // ⚡ MOVEMENT & SPEED HACKS
        // ==========================================
 
        internal static bool SpeedEnabled = false;
        internal static bool SpeedHackEnabled = false;
        internal static bool speedHackEnabled = false;
        internal static bool SpeedJoystick = false;
        internal static float speedMultiplier = 1.0f;

        internal static bool flyme = false;
       
        internal static float FlyHeight = 5;
        internal static bool JumpUp = false;
        internal static bool ClimbUp = false;

        internal static bool UpPlayer1 = false;
        internal static bool Downplayer = false;
        internal static bool DownPlayer1 = false;
        internal static bool ghoston = false;
        internal static bool ghostoff = false;

        internal static bool MagicBullet = false;
        internal static bool FastWeaponSwitch = false;
        internal static bool AutoFireEnabled = false;
        internal static float AutoFireMaxDistance = 360f;

        internal static bool SniperScope = false;
        internal static Keys SniperScope1 = Keys.LButton;

        internal static bool Aimkill = false;
        internal static bool AimKillEnabled = true;
        internal static bool Shakekill = false;
        internal static bool spawnkill = false;
        internal static bool proxtelekill = false;
        internal static bool hamba = false;
        internal static Keys sowansaskillas = Keys.LButton;

        internal static int ShakeKillPower = 1;
        internal static int shakekilllsliddr = 0;



        // ==========================================
        // 🛠️ SYSTEM, VISUALS & CROSSHAIR
        // ==========================================
        internal static bool CrosshairEnabled = false;
        internal static bool CrosshairEnabledRGB = false;
        internal static Color CrosshairColor = Color.White;
        internal static float CrosshairSize = 15f;
        internal static float CrosshairThickness = 2f;
        internal static bool DrawShurikenCrosshair = false;
        internal static float CrosshairRotationSpeed = 2f;

        internal static int DrawShurikenCrosshairSpeed = 1000;

        internal static bool PARTICLE_OFF = false;
        internal static float GlowRadius = 15;
        internal static float FeatherAmount = 2f;
        internal static float GlowOpacity = 0.02f;
        internal static bool ImageGlow = false;
        internal static bool espbg = false;
        internal static bool espcfx = false;

        internal static bool CameraHackEnabled = false;
        internal static bool sound = false;
        internal static bool TeamCheck = true;
        internal static bool VisibilityCheck = true;

        internal static int LagFix = 0;
        internal static int thread = 0;
        internal static int expsize = 8;

        // Transformation & Offsets
        internal static bool TransformHackEnabled = false;
        internal static bool TransformYEnabled = true;
        internal static bool TransformXEnabled = false;
        internal static bool TransformZEnabled = false;
        internal static int TransformMode = 0;
        internal static float YOffset = 0.030f;
        internal static float YOffset1 = 5.0f;
        internal static float XOffset = 0f;
        internal static float ZOffset = 0f;
        internal static float TargetHeight = 100.0f;
        internal static float LiftAmount = 10.0f;
        internal static float MovementSpeed = 1.0f;
        internal static bool SmoothMovement = true;
        internal static int SliderY = -3;
        internal static int Sliderx = 1;


        internal static Color FOVColor = Color.White;
        internal static LinePosition ESPLinePosition = LinePosition.Top;

        internal static Vector2 EspWeaponIconOffsetPx = Vector2.Zero;
        internal static Vector2 EspNameOffsetPx = Vector2.Zero;
        internal static Vector2 EspHackerOffsetPx = Vector2.Zero;
        internal static Vector2 EspBoxOffsetPx = Vector2.Zero;
        internal static Vector2 EspHealthBarOffsetPx = Vector2.Zero;
        internal static Vector2 EspSnapLineStartOffsetPx = Vector2.Zero;
        internal static Vector2 EspSnapLineEndOffsetPx = Vector2.Zero;
        /// <summary>Fine offset for distance text when it is drawn outside the nameplate (editor + in-game).</summary>
        internal static Vector2 EspDistanceOffsetPx = Vector2.Zero;

        internal static EspNameBoxAnchor EspNameAnchor = EspNameBoxAnchor.BoxTopCenter;
        internal static EspDistanceBoxAnchor EspDistanceAnchor = EspDistanceBoxAnchor.InsideNameplate;
        internal static EspHealthBoxAnchor EspHealthAnchor = EspHealthBoxAnchor.BoxRight;
        internal static EspWeaponBoxAnchor EspWeaponAnchor = EspWeaponBoxAnchor.BoxBottomCenter;

        internal static float cameraVal = 1.0f;
        internal static float visionVal = 3.141592741f;

        private const string ProfileFileExt = ".profile.cfg";

        /// <summary>Fixed slot — one save file only (no named profiles).</summary>
        internal const string SingleConfigSlotName = "config";

        internal static string ProfilesDirectory
        {
            get
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string dir = Path.Combine(baseDir, "Profiles");
                Directory.CreateDirectory(dir);
                return dir;
            }
        }

        internal static string SingleConfigFilePath => GetProfilePath(SingleConfigSlotName);

        internal static bool SaveSingleConfig(out string error) => SaveProfile(SingleConfigSlotName, out error);

        internal static bool LoadSingleConfig(out string error) => LoadProfile(SingleConfigSlotName, out error);

        internal static bool DeleteSingleConfig(out string error) => DeleteProfile(SingleConfigSlotName, out error);

        internal static bool SingleConfigExists() => File.Exists(SingleConfigFilePath);

        /// <summary>Reads <see cref="AutoLoadConfig"/> from disk, then loads full config if it is true.</summary>
        internal static void ApplyStartupFromSingleConfigFile()
        {
            if (!File.Exists(SingleConfigFilePath))
                return;
            try
            {
                var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (string line in File.ReadAllLines(SingleConfigFilePath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    int sep = line.IndexOf('=');
                    if (sep <= 0)
                        continue;
                    map[line[..sep].Trim()] = line[(sep + 1)..].Trim();
                }
                if (map.TryGetValue(nameof(AutoLoadConfig), out string? raw) &&
                    TryDeserializeValue(raw, typeof(bool), out object? v) && v is bool b)
                    AutoLoadConfig = b;
                if (AutoLoadConfig)
                    LoadSingleConfig(out _);
            }
            catch
            {
                /* ignore malformed startup file */
            }
        }

        private static bool SaveProfile(string profileName, out string error)
        {
            error = "";
            string normalized = NormalizeProfileName(profileName);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                error = "Profile name is empty.";
                return false;
            }

            try
            {
                string path = GetProfilePath(normalized);
                var lines = new List<string>();
                foreach (var f in GetPersistedFields())
                {
                    object? value = f.GetValue(null);
                    lines.Add($"{f.Name}={SerializeValue(value, f.FieldType)}");
                }
                File.WriteAllLines(path, lines);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static bool LoadProfile(string profileName, out string error)
        {
            error = "";
            string normalized = NormalizeProfileName(profileName);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                error = "Profile name is empty.";
                return false;
            }

            string path = GetProfilePath(normalized);
            if (!File.Exists(path))
            {
                error = "Profile file not found.";
                return false;
            }

            try
            {
                var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (string line in File.ReadAllLines(path))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    int sep = line.IndexOf('=');
                    if (sep <= 0)
                        continue;
                    string key = line[..sep].Trim();
                    string value = line[(sep + 1)..].Trim();
                    map[key] = value;
                }

                foreach (var f in GetPersistedFields())
                {
                    if (!map.TryGetValue(f.Name, out string? raw))
                        continue;
                    if (TryDeserializeValue(raw, f.FieldType, out object? parsed))
                        f.SetValue(null, parsed);
                }
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static bool DeleteProfile(string profileName, out string error)
        {
            error = "";
            string normalized = NormalizeProfileName(profileName);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                error = "Profile name is empty.";
                return false;
            }

            string path = GetProfilePath(normalized);
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static IEnumerable<FieldInfo> GetPersistedFields()
        {
            return typeof(Config)
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => !f.IsLiteral && !f.IsInitOnly);
        }

        private static string GetProfilePath(string normalizedName)
        {
            return Path.Combine(ProfilesDirectory, normalizedName + ProfileFileExt);
        }

        private static string NormalizeProfileName(string profileName)
        {
            string n = profileName?.Trim() ?? "";
            if (n.Length == 0)
                return "";
            foreach (char c in Path.GetInvalidFileNameChars())
                n = n.Replace(c.ToString(), "");
            return n.Trim();
        }

        private static string SerializeValue(object? value, Type t)
        {
            if (value == null)
                return "";
            if (t == typeof(bool))
                return (bool)value ? "true" : "false";
            if (t == typeof(int))
                return ((int)value).ToString(CultureInfo.InvariantCulture);
            if (t == typeof(float))
                return ((float)value).ToString("R", CultureInfo.InvariantCulture);
            if (t.IsEnum)
                return value.ToString() ?? "";
            if (t == typeof(Color))
            {
                Color c = (Color)value;
                return $"{c.A},{c.R},{c.G},{c.B}";
            }
            if (t == typeof(Vector2))
            {
                var v = (Vector2)value;
                return $"{v.X.ToString("R", CultureInfo.InvariantCulture)},{v.Y.ToString("R", CultureInfo.InvariantCulture)}";
            }
            if (t == typeof(Vector4))
            {
                var v = (Vector4)value;
                return string.Join(",",
                    v.X.ToString("R", CultureInfo.InvariantCulture),
                    v.Y.ToString("R", CultureInfo.InvariantCulture),
                    v.Z.ToString("R", CultureInfo.InvariantCulture),
                    v.W.ToString("R", CultureInfo.InvariantCulture));
            }
            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
        }

        private static bool TryDeserializeValue(string raw, Type t, out object? value)
        {
            value = null;
            if (t == typeof(bool))
            {
                if (bool.TryParse(raw, out bool b)) { value = b; return true; }
                return false;
            }
            if (t == typeof(int))
            {
                if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i)) { value = i; return true; }
                return false;
            }
            if (t == typeof(float))
            {
                if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float f)) { value = f; return true; }
                return false;
            }
            if (t.IsEnum)
            {
                try { value = Enum.Parse(t, raw, true); return true; } catch { return false; }
            }
            if (t == typeof(Color))
            {
                string[] p = raw.Split(',');
                if (p.Length == 4 &&
                    byte.TryParse(p[0], out byte a) &&
                    byte.TryParse(p[1], out byte r) &&
                    byte.TryParse(p[2], out byte g) &&
                    byte.TryParse(p[3], out byte b))
                {
                    value = Color.FromArgb(a, r, g, b);
                    return true;
                }
                return false;
            }
            if (t == typeof(Vector2))
            {
                string[] p = raw.Split(',');
                if (p.Length == 2 &&
                    float.TryParse(p[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                    float.TryParse(p[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
                {
                    value = new Vector2(x, y);
                    return true;
                }
                return false;
            }
            if (t == typeof(Vector4))
            {
                string[] p = raw.Split(',');
                if (p.Length == 4 &&
                    float.TryParse(p[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                    float.TryParse(p[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                    float.TryParse(p[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z) &&
                    float.TryParse(p[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float w))
                {
                    value = new Vector4(x, y, z, w);
                    return true;
                }
                return false;
            }
            return false;
        }
    }
    public enum TargetingMode
    {
        ClosestToCrosshair,
        Target360,
        ClosestToPlayer,
        LowestHealth,
    }
    public enum AimBotType
    {
        Silent,
        Rage
    }
    public enum LinePosition
    {
        Top,
        Center,
        Bottom
    }

    /// <summary>Where ESP name / nameplate sits relative to the corner ESP box.</summary>
    public enum EspNameBoxAnchor
    {
        BoxTopCenter,
        BoxBottomCenter,
        BoxTopLeft,
        BoxTopRight,
        BoxBottomLeft,
        BoxBottomRight,
        BoxMiddleLeft,
        BoxMiddleRight,
    }

    /// <summary>Where distance text is drawn (ESP Information). InsideNameplate = original bar layout.</summary>
    public enum EspDistanceBoxAnchor
    {
        InsideNameplate,
        AboveNameWhenNameAtTop,
        BoxTopCenter,
        BoxBottomCenter,
        BoxTopLeft,
        BoxTopRight,
        BoxBottomLeft,
        BoxBottomRight,
        BoxMiddleLeft,
        BoxMiddleRight,
    }

    public enum EspHealthBoxAnchor
    {
        BoxRight,
        BoxLeft,
        BoxTop,
        BoxBottom,
    }

    public enum EspWeaponBoxAnchor
    {
        BoxTopCenter,
        BoxBottomCenter,
        BoxTopLeft,
        BoxTopRight,
        BoxBottomLeft,
        BoxBottomRight,
        BoxMiddleLeft,
        BoxMiddleRight,
    }
   
}
