using AotForms;
using ImGuiNET;
using Newtonsoft.Json.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using static AotForms.WinAPI;
using static TheArtOfDevHtmlRenderer.Adapters.RGraphicsPath;

namespace AotForms
{
    internal class ESP : ClickableTransparentOverlay.Overlay
    {
        IntPtr hWnd;
        IntPtr HDPlayer;
        private const short DefaultMaxHealth = 200; // Default maximum health

        public static class Renderer
        {
            public static bool WorldToScreen(Vector3 worldPos, out Vector2 screenPos)
            {
                screenPos = new Vector2(worldPos.X, worldPos.Y);
                return true; 
            }
        }
        public static bool WorldToScreen(Vector3 worldPos, out Vector2 screenPos)
        {
            screenPos = Vector2.Zero;

            Matrix4x4 viewMatrix = Core.ViewMatrix;

            Vector4 clipCoords = new Vector4(
                worldPos.X * viewMatrix.M11 + worldPos.Y * viewMatrix.M21 + worldPos.Z * viewMatrix.M31 + viewMatrix.M41,
                worldPos.X * viewMatrix.M12 + worldPos.Y * viewMatrix.M22 + worldPos.Z * viewMatrix.M32 + viewMatrix.M42,
                worldPos.X * viewMatrix.M13 + worldPos.Y * viewMatrix.M23 + worldPos.Z * viewMatrix.M33 + viewMatrix.M43,
                worldPos.X * viewMatrix.M14 + worldPos.Y * viewMatrix.M24 + worldPos.Z * viewMatrix.M34 + viewMatrix.M44
            );

            if (clipCoords.W < 0.1f)
                return false;

            // perspective division
            Vector3 ndc;
            ndc.X = clipCoords.X / clipCoords.W;
            ndc.Y = clipCoords.Y / clipCoords.W;
            ndc.Z = clipCoords.Z / clipCoords.W;

            // screen space coords
            screenPos.X = (Core.Width / 2f) * (ndc.X + 1f);
            screenPos.Y = (Core.Height / 2f) * (1f - ndc.Y); // flip Y

            return true;
        }

        protected override unsafe void Render()
        {
            ImGui.GetForegroundDrawList().AddText(new Vector2(Core.Width / 2f - 40, 50), ColorToUint32(Config.NameCheat), "");
            ImGui.GetForegroundDrawList().AddText(new Vector2(Core.Width / 2f - 40, 70), ColorToUint32(Config.NameCheat), "");
            if (!Core.HaveMatrix) return;

            CreateHandle();
            string text = "</>DEV: KRISHU X CHEATS";
            var windowWidth = Core.Width;
            var windowHeight = Core.Height;

            var drawList = ImGui.GetForegroundDrawList();


            if (Config.Aimfovc)
            {
                // Draw a single glow layer
                uint glowColor = ColorToUint32(Color.FromArgb((int)(1f * 255), Config.Aimfovcolor));

                DrawSmoothCircle(Config.AimBotFov, glowColor, 1.0f);
            }

            var tmp = Core.Entities;

            // Handle window styles
            string windowName = "Overlay";
            hWnd = FindWindow(null, windowName);
            HDPlayer = FindWindow("BlueStacksApp", null);

            if (hWnd != IntPtr.Zero)
            {
                long extendedStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
                SetWindowLong(hWnd, GWL_EXSTYLE, (extendedStyle | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
            }
            else
            {
                Console.WriteLine("The window was not found.");
            }



            Vector2 sharedCirclePosition = new Vector2(0, 0);

            sharedCirclePosition = new Vector2(windowWidth / 2f, 30);

            int enemyCount = 0;
            if (Config.minimap)
            {
                DrawMinimap();
            }

            foreach (var entity in tmp.Values)
            {
                if (entity.IsDead || !entity.IsKnown)
                {
                    continue;
                }

                var dist = Vector3.Distance(Core.LocalMainCamera, entity.Head);

                if (dist > Config.espran) continue;
                enemyCount++;
                var headScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.Head, Core.Width, Core.Height);

                var bottomScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.Root, Core.Width, Core.Height);

                if (headScreenPos.X < 1 || headScreenPos.Y < 1) continue;
                if (bottomScreenPos.X < 1 || bottomScreenPos.Y < 1) continue;

                float CornerHeight = Math.Abs(headScreenPos.Y - bottomScreenPos.Y);
                float CornerWidth = (float)(CornerHeight * 0.65);

                if (Config.ESPLine)
                {
                    // Check if the entity is "Knocked"
                    uint lineColor;

                    if (entity.IsKnocked)
                    {
                        lineColor = ColorToUint32(Color.Red); // Red for "Knocked" state
                    }
                    else
                    {
                        lineColor = ColorToUint32(Config.ESPLineColor); // Normal color
                    }

                    // Draw the line with the appropriate color
                    ImGui.GetBackgroundDrawList().AddLine(
                        new Vector2(Core.Width / 2f, 0f),
                        headScreenPos,
                        lineColor,
                        1f
                    );
                }



                if (Config.ESPclosest && Core.Entities.Count > 0)
                {
                    Entity closestEntity = null;
                    float closestDistance = float.MaxValue;

                    foreach (var e in Core.Entities.Values)
                    {
                        if (e == null) continue;
                        if (e.IsDead || e.IsKnocked || !e.isVisible) continue;

                        if (e.Distance < closestDistance)
                        {
                            closestDistance = e.Distance;
                            closestEntity = e;
                        }
                    }

                    if (closestEntity != null)
                    {
                        Vector2 closestHeadScreenPos;
                        if (WorldToScreen(closestEntity.Head, out closestHeadScreenPos))
                        {
                            Vector2 lineStart = new Vector2(Core.Width / 2f, Core.Height / 2f); // crosshair center
                            Vector2 lineEnd = new Vector2(closestHeadScreenPos.X, closestHeadScreenPos.Y);
                            float increasedGlowRadius = Config.GlowRadius * 2f;

                            DrawGlowLine(lineStart, lineEnd, ColorToUint32(Config.ESPLineColor), 1f, increasedGlowRadius, Config.FeatherAmount, Config.GlowOpacity);
                        }
                    }
                }











                if (Config.ESPFillBox)
                {
                    uint boxColor = ColorToUint32(Color.FromArgb((int)(0.2f * 255), Config.ESPFillBoxColor));
                    DrawFilledBox(headScreenPos.X - (CornerWidth / 2), headScreenPos.Y, CornerWidth, CornerHeight, boxColor);
                }


                if (Config.ESPBox2)
                {
                    uint boxColor = ColorToUint32(Config.ESPBoxColor);

                    // Define glow parameters for 3D box
                    float glowRadius = 15f; // Adjust the glow radius as needed
                    float feather = 1.7f; // Feather effect for the glow
                    float glowOpacityMultiplier = 0.02f; // Glow opacity control

                    Draw3dBox(headScreenPos.X - (CornerWidth / 2), headScreenPos.Y, CornerWidth, CornerHeight, boxColor, 1f, Config.BoxGlow ? 1f : 0f, feather, glowOpacityMultiplier);
                }

                if (Config.ESPBox)
                {
                    uint boxColor = ColorToUint32(Config.ESPBoxColor);

                    DrawCorneredBox(headScreenPos.X - (CornerWidth / 2), headScreenPos.Y, CornerWidth, CornerHeight, boxColor, 1f);
                }

                var nameText = string.IsNullOrWhiteSpace(entity.Name) ? "BOT" : entity.Name;
                var namePosition = new Vector2(headScreenPos.X - (CornerWidth / 2), headScreenPos.Y - 36);

                // Fixed dimensions for ESP background
                float fixedBgWidth = 125.0f;
                float fixedBgHeight = 18.0f;
                if (Config.ESPName)
                {
                    // Define background boundaries
                    Vector2 bgCenter = namePosition - new Vector2(6, 2);
                    Vector2 bgBottomRight = namePosition + new Vector2(fixedBgWidth, fixedBgHeight);

                    // Draw the background with black 30% opacity
                    drawList.AddRectFilled(
                        bgCenter,
                        bgBottomRight,
                        ColorToUint32(Color.FromArgb(55, 0, 0, 0)) // 80% opacity
                    );

                    // ESP Outline: Soft dark edge
                    drawList.AddRect(
                        bgCenter,
                        bgBottomRight,
                        ColorToUint32(Color.FromArgb(120, 30, 20, 10)), // dark brown/blackish outline
                        0.0f,
                        ImDrawFlags.None,
                        0.7f // Slightly more visible
                    );
                }
                if (Config.ESPName)
                {
                    // Prepare text strings.
                    string distanceText = $"{MathF.Round(dist)}M".ToUpper();
                    string healthText = $"{entity.Health}HP".ToUpper(); // Health text appended with "HP"

                    // Calculate text sizes for positioning.
                    Vector2 distanceSize = ImGui.CalcTextSize(distanceText);
                    Vector2 calculatedNameSize = ImGui.CalcTextSize(nameText);
                    Vector2 healthSize = ImGui.CalcTextSize(healthText);

                    // Calculate the background boundaries.
                    float bgLeft = namePosition.X - 8;
                    float bgRight = bgLeft + fixedBgWidth;
                    float bgCenterX = (bgLeft + bgRight) / 2;

                    // For vertical centering within the background.
                    float centerY = namePosition.Y;

                    // Use a horizontal padding value.
                    float paddingX = 6;

                    // Set positions relative to the background:
                    Vector2 distancePosition = new Vector2(bgLeft + paddingX, centerY);               // Left edge
                    Vector2 namePositionAdjusted = new Vector2(bgCenterX - (calculatedNameSize.X / 2), centerY); // Centered
                    Vector2 healthPosition = new Vector2(bgRight - healthSize.X - paddingX, centerY);     // Right edge

                    // Define text colors:
                    uint pureYellow = ColorToUint32(Color.FromArgb(255, 255, 255, 0));  // For distance text
                    uint pureWhite = ColorToUint32(Color.FromArgb(255, 255, 255, 255)); // For name text
                    uint greenText = ColorToUint32(Color.FromArgb(255, 0, 255, 0));       // For health text

                    // Draw the texts:
                    drawList.AddText(distancePosition, pureYellow, distanceText); // Distance text on the left
                    drawList.AddText(namePositionAdjusted, pureWhite, nameText);    // Name text in the center
                    drawList.AddText(healthPosition, greenText, healthText);        // Health text (green) on the right
                }





                if (Config.ESPHealth)
                {
                    float healthBarHeight = 4;

                    // Health bar width should match the background outline width (with a slight extension)
                    float healthBarWidth = fixedBgWidth + 8;

                    // Position the health bar exactly below the ESP background
                    var healthBarPosition = new Vector2(namePosition.X - 8, namePosition.Y + fixedBgHeight);

                    // Draw the health bar based on whether the entity is knocked or not.
                    if (!entity.IsKnocked)
                    {
                        DrawHealthBar(entity.Health, DefaultMaxHealth, healthBarPosition.X, healthBarPosition.Y, healthBarHeight, healthBarWidth);
                    }
                    else
                    {
                        DrawHealthBar(entity.Health, DefaultMaxHealth, healthBarPosition.X, healthBarPosition.Y, healthBarHeight, healthBarWidth);
                    }
                }

                if (Config.ESPSkeleton)
                {
                    DrawSkeleton(entity);
                }

                if (Config.ESPWeapon)
                {
                    DrawWeaponTextESP(entity, headScreenPos, CornerHeight);
                }

                if (Config.ESPWeaponIcon)
                {
                    Vector2 fixedNameSize = new Vector2(95, 16);

                    if (headScreenPos.X >= 0 && headScreenPos.Y >= 0 && headScreenPos.X <= Core.Width && headScreenPos.Y <= Core.Height)
                    {
                        Vector2 namePos = new Vector2(headScreenPos.X - fixedNameSize.X / 2, headScreenPos.Y - fixedNameSize.Y - 15);

                        var imagepath = $"D:\\FF Panel Project\\Aimbot\\AotForms\\WeaponIcon\\{entity.WeaponName.ToLower()}.png";


                        try
                        {
                            IntPtr imagehandle;
                            AddOrGetImagePointer(imagepath, true, out imagehandle, out var width, out var height);
                            {
                                Vector2 iconSize = new Vector2(60, 20);
                                Vector2 iconPos = new Vector2(namePos.X + (fixedNameSize.X - iconSize.X) / 2, namePos.Y - iconSize.Y - 2);
                                ImGui.GetForegroundDrawList().AddImage(imagehandle, iconPos, iconPos + iconSize);
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        private void DrawMinimap()
        {
            var windowWidth = Core.Width;
            var windowHeight = Core.Height;


            int DetectionRange = 250;


            float minimapWidth = 200f * (DetectionRange / 250f);
            float minimapHeight = 150f * (DetectionRange / 250f);
            Vector2 minimapCenter = new Vector2(20 + minimapWidth / 2, windowHeight - minimapHeight / 2 - 20);


            float cameraYaw = -GetCameraYaw();
            float cosYaw = MathF.Cos(cameraYaw);
            float sinYaw = MathF.Sin(cameraYaw);


            ImDrawListPtr drawList = ImGui.GetBackgroundDrawList();
            uint minimapBackgroundColor = ColorToUint32(Color.FromArgb(180, 10, 10, 10));
            uint minimapBorderColor = ColorToUint32(Color.FromArgb(220, 255, 255, 255));

            float borderRadius = 2f;

            drawList.AddRectFilled(minimapCenter - new Vector2(minimapWidth / 2, minimapHeight / 2),
                                   minimapCenter + new Vector2(minimapWidth / 2, minimapHeight / 2),
                                   minimapBackgroundColor, borderRadius);

            float borderThickness = 0.3f;
            drawList.AddRect(minimapCenter - new Vector2(minimapWidth / 2, minimapHeight / 2),
                             minimapCenter + new Vector2(minimapWidth / 2, minimapHeight / 2),
                             minimapBorderColor, borderRadius, ImDrawFlags.None, borderThickness);


            uint gridColor = ColorToUint32(Color.FromArgb(50, 255, 255, 255));
            for (float x = -minimapWidth / 2; x <= minimapWidth / 2; x += minimapWidth / 4)
            {
                drawList.AddLine(minimapCenter + new Vector2(x, -minimapHeight / 2),
                                 minimapCenter + new Vector2(x, minimapHeight / 2),
                                 gridColor, 0.5f);
            }
            for (float y = -minimapHeight / 2; y <= minimapHeight / 2; y += minimapHeight / 4)
            {
                drawList.AddLine(minimapCenter + new Vector2(-minimapWidth / 2, y),
                                 minimapCenter + new Vector2(minimapWidth / 2, y),
                                 gridColor, 0.5f);
            }


            string[] compassDirections = { "N", "E", "S", "W" };
            for (int i = 0; i < 4; i++)
            {
                float angle = i * MathF.PI / 2;
                Vector2 directionPos = minimapCenter + new Vector2(MathF.Cos(angle) * minimapWidth / 2 * 0.7f, -MathF.Sin(angle) * minimapHeight / 2 * 0.7f);
                drawList.AddText(directionPos, ColorToUint32(Color.White), compassDirections[i]);
            }


            uint playerColor = ColorToUint32(Color.Cyan);
            drawList.AddCircleFilled(minimapCenter, 6.0f, playerColor);


            foreach (var entity in Core.Entities.Values)
            {
                if (entity.IsDead) continue;

                float distance = Vector3.Distance(Core.LocalMainCamera, entity.Head);


                if (distance > DetectionRange) continue;


                Vector3 relativePosition = entity.Head - Core.LocalMainCamera;
                float scale = minimapWidth / (float)DetectionRange;


                float rotatedX = relativePosition.X * cosYaw - relativePosition.Z * sinYaw;
                float rotatedY = relativePosition.X * sinYaw + relativePosition.Z * cosYaw;


                Vector2 enemyOnMinimap = new Vector2(minimapCenter.X + rotatedX * scale, minimapCenter.Y - rotatedY * scale);


                if (enemyOnMinimap.X >= minimapCenter.X - minimapWidth / 2 && enemyOnMinimap.X <= minimapCenter.X + minimapWidth / 2 &&
                    enemyOnMinimap.Y >= minimapCenter.Y - minimapHeight / 2 && enemyOnMinimap.Y <= minimapCenter.Y + minimapHeight / 2)
                {

                    uint enemyColor = entity.IsKnown
                        ? (entity.IsKnocked ? ColorToUint32(Color.Yellow) : ColorToUint32(Color.Red))
                        : ColorToUint32(Color.Blue);


                    drawList.AddCircleFilled(enemyOnMinimap, 4.0f, enemyColor);
                }
            }
        }


        private float GetCameraYaw()
        {
            return MathF.Atan2(Core.CameraMatrix.M31, Core.CameraMatrix.M33);
        }

        void DrawTextWithOutline(Vector2 pos, string text, uint textColor, uint outlineColor)
        {
            var vList = ImGui.GetForegroundDrawList();

            // Desenhar contorno
            vList.AddText(new Vector2(pos.X - 1, pos.Y), outlineColor, text);
            vList.AddText(new Vector2(pos.X + 1, pos.Y), outlineColor, text);
            vList.AddText(new Vector2(pos.X, pos.Y - 1), outlineColor, text);
            vList.AddText(new Vector2(pos.X, pos.Y + 1), outlineColor, text);

            // Desenhar texto principal
            vList.AddText(pos, textColor, text);
        }

        public void DrawGlowLine(Vector2 start, Vector2 end, uint color, float thickness, float glowRadius, float feather, float glowOpacityMultiplier)
        {
            var drawList = ImGui.GetBackgroundDrawList();
            Vector4 colorVec = ImGui.ColorConvertU32ToFloat4(color);

            // Outer glow layers for the line
            for (float i = glowRadius; i > 0; i -= feather)
            {
                float alpha = colorVec.W * (i / glowRadius) * glowOpacityMultiplier;
                alpha = Clamp(alpha, 0, 1);

                uint glowColor = ImGui.ColorConvertFloat4ToU32(new Vector4(colorVec.X, colorVec.Y, colorVec.Z, alpha));

                // Draw the glow for the line
                drawList.AddLine(start, end, glowColor, thickness + (glowRadius - i) * 0.5f);
            }

            // Main line at the center
            drawList.AddLine(start, end, color, thickness);

            // Draw start and end glows as circles
            for (float i = glowRadius; i > 0; i -= feather)
            {
                float alpha = colorVec.W * (i / glowRadius) * glowOpacityMultiplier;
                alpha = Clamp(alpha, 0, 1);

                uint glowColor = ImGui.ColorConvertFloat4ToU32(new Vector4(colorVec.X, colorVec.Y, colorVec.Z, alpha));

                // Draw circles at the start and end points
                float radius = thickness / 2 + (glowRadius - i) * 0.5f;
                drawList.AddCircleFilled(start, radius, glowColor);
                drawList.AddCircleFilled(end, radius, glowColor);
            }

            // Draw rounded corners for the main line
            drawList.AddCircleFilled(start, thickness / 2, color);
            drawList.AddCircleFilled(end, thickness / 2, color);
        }

        // Custom clamp function
        private float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }


        void DrawWeaponTextESP(Entity entity, Vector2 headScreenPos, float boxHeight)
        {
            if (!Config.ESPWeapon || entity == null || string.IsNullOrEmpty(entity.WeaponName))
                return;

            Vector2 weaponTextPos = new Vector2(
                headScreenPos.X - ImGui.CalcTextSize(entity.WeaponName).X / 2,
                headScreenPos.Y + boxHeight + 5
            );

            uint textColor = ColorToUint32(Color.White);
            ImGui.GetForegroundDrawList().AddText(weaponTextPos, textColor, entity.WeaponName);
        }

        public void DrawGradientBox(float X, float Y, float W, float H, Color topColor, Color bottomColor)
        {
            var vList = ImGui.GetForegroundDrawList();

            int slices = 50; // Number of slices for gradient
            float sliceHeight = H / slices;

            for (int i = 0; i < slices; i++)
            {
                float t = (float)i / slices; // Interpolation factor
                Color sliceColor = Color.FromArgb(
                    (int)(topColor.A * (1 - t) + bottomColor.A * t), // Interpolating opacity
                    (int)(topColor.R * (1 - t) + bottomColor.R * t), // Interpolating Red
                    (int)(topColor.G * (1 - t) + bottomColor.G * t), // Interpolating Green
                    (int)(topColor.B * (1 - t) + bottomColor.B * t)  // Interpolating Blue
                );

                uint sliceColorUint = ColorToUint32(sliceColor);

                // Draw each slice
                vList.AddRectFilled(
                    new Vector2(X, Y + i * sliceHeight),
                    new Vector2(X + W, Y + (i + 1) * sliceHeight),
                    sliceColorUint
                );
            }
        }


        public void DrawFilledCircle(float centerY, float radius, int numSegments = 64)
        {
            var vList = ImGui.GetBackgroundDrawList();

            // Set the center of the circle at the middle of the screen horizontally (Core.Width / 2f)

            float centerX = Core.Width / 2f;

            uint colorR = ColorToUint32(Color.FromArgb((int)(1f * 255), 225, 0, 0)); // Red color with full opacity
            uint colorG = ColorToUint32(Color.FromArgb((int)(1f * 255), 0, 255, 0)); // LimeGreen color with full opacity

            // Shadow parameters
            float shadowOffset = 1.08f; // The subtle offset of the shadow from the circle
            uint shadowColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0f, 0f, 1f)); // Semi-transparent black for a soft shadow

            // Draw shadow (a larger circle slightly offset behind the main one)
            vList.AddCircleFilled(new Vector2(centerX, centerY), radius + shadowOffset, shadowColor, numSegments);

            if (Config.AimBot)
            {
                // Draw main circle
                vList.AddCircleFilled(new Vector2(centerX, centerY), radius, colorR, numSegments);
            }
            else
            {
                // Draw main circle
                vList.AddCircleFilled(new Vector2(centerX, centerY), radius, colorG, numSegments);
            }
        }

        public void DrawFilledBox(float X, float Y, float W, float H, uint color)
        {
            var vList = ImGui.GetForegroundDrawList();
            vList.AddRectFilled(new Vector2(X, Y), new Vector2(X + W, Y + H), color);
        }
        public void DrawCorneredBox(float X, float Y, float W, float H, uint color, float thickness)
        {
            var vList = ImGui.GetForegroundDrawList();

            float lineW = W / 3;
            float lineH = H / 3;

            vList.AddLine(new Vector2(X, Y - thickness / 2), new Vector2(X, Y + lineH), color, thickness);
            vList.AddLine(new Vector2(X - thickness / 2, Y), new Vector2(X + lineW, Y), color, thickness);
            vList.AddLine(new Vector2(X + W - lineW, Y), new Vector2(X + W + thickness / 2, Y), color, thickness);
            vList.AddLine(new Vector2(X + W, Y - thickness / 2), new Vector2(X + W, Y + lineH), color, thickness);
            vList.AddLine(new Vector2(X, Y + H - lineH), new Vector2(X, Y + H + thickness / 2), color, thickness);
            vList.AddLine(new Vector2(X - thickness / 2, Y + H), new Vector2(X + lineW, Y + H), color, thickness);
            vList.AddLine(new Vector2(X + W - lineW, Y + H), new Vector2(X + W + thickness / 2, Y + H), color, thickness);
            vList.AddLine(new Vector2(X + W, Y + H - lineH), new Vector2(X + W, Y + H + thickness / 2), color, thickness);
        }





        public void Draw3dBox(float X, float Y, float W, float H, uint color, float thickness, float glowRadius, float feather, float glowOpacityMultiplier)
        {
            var vList = ImGui.GetForegroundDrawList();
            Vector4 colorVec = ImGui.ColorConvertU32ToFloat4(color);

            Vector3[] screentions = new Vector3[]
            {
        new Vector3(X, Y, 0),
        new Vector3(X, Y + H, 0),
        new Vector3(X + W, Y + H, 0),
        new Vector3(X + W, Y, 0),
        new Vector3(X, Y, -W),
        new Vector3(X, Y + H, -W),
        new Vector3(X + W, Y + H, -W),
        new Vector3(X + W, Y, -W)
            };

            // Draw glow effect for each line and add circles at start and end
            for (float i = glowRadius; i > 0; i -= feather)
            {
                float alpha = colorVec.W * (i / glowRadius) * glowOpacityMultiplier;
                alpha = Clamp(alpha, 0, 1);
                uint glowColor = ImGui.ColorConvertFloat4ToU32(new Vector4(colorVec.X, colorVec.Y, colorVec.Z, alpha));

                float currentThickness = thickness + (glowRadius - i);

                // Front face with circle glow at start and end
                DrawBoxLinesWithGlow(vList, screentions, glowColor, currentThickness, new int[] { 0, 1, 2, 3 });
                AddGlowCircles(vList, screentions[0], currentThickness, glowColor);
                AddGlowCircles(vList, screentions[3], currentThickness, glowColor);

                // Back face with circle glow at start and end
                DrawBoxLinesWithGlow(vList, screentions, glowColor, currentThickness, new int[] { 4, 5, 6, 7 });
                AddGlowCircles(vList, screentions[4], currentThickness, glowColor);
                AddGlowCircles(vList, screentions[7], currentThickness, glowColor);

                // Connecting lines with circle glow at start and end
                for (int j = 0; j < 4; j++)
                {
                    vList.AddLine(new Vector2(screentions[j].X, screentions[j].Y), new Vector2(screentions[j + 4].X, screentions[j + 4].Y), glowColor, currentThickness);
                    AddGlowCircles(vList, screentions[j], currentThickness, glowColor);
                    AddGlowCircles(vList, screentions[j + 4], currentThickness, glowColor);
                }
            }
            DrawBoxLinesWithGlow(vList, screentions, color, thickness, new int[] { 0, 1, 2, 3 }); // Front face
            DrawBoxLinesWithGlow(vList, screentions, color, thickness, new int[] { 4, 5, 6, 7 }); // Back face

            for (int j = 0; j < 4; j++)
            {
                vList.AddLine(new Vector2(screentions[j].X, screentions[j].Y), new Vector2(screentions[j + 4].X, screentions[j + 4].Y), color, thickness);
                AddGlowCircles(vList, screentions[j], thickness / 2, color);
                AddGlowCircles(vList, screentions[j + 4], thickness / 2, color);
            }
        }
        private void AddGlowCircles(ImDrawListPtr vList, Vector3 position, float radius, uint glowColor)
        {
            vList.AddCircleFilled(new Vector2(position.X, position.Y), radius, glowColor);
        }
        private void DrawBoxLinesWithGlow(ImDrawListPtr vList, Vector3[] points, uint color, float thickness, int[] indices)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                int start = indices[i];
                int end = indices[(i + 1) % indices.Length];
                vList.AddLine(new Vector2(points[start].X, points[start].Y), new Vector2(points[end].X, points[end].Y), color, thickness);
            }
        }


        private void DrawSkeleton(Entity entity)
        {
            var drawList = ImGui.GetForegroundDrawList();
            uint lineColor = ColorToUint32(Config.ESPSkeletonColor); // Color for the skeleton lines
            uint circleColor = ColorToUint32(Color.Red); // Color for the circle around the head

            // Convert entity positions to screen space
            var headScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.Head, Core.Width, Core.Height);
            var leftWristScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightWrist, Core.Width, Core.Height); // Adjust as per actual mapping
            var spineScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.Spine, Core.Width, Core.Height);
            var hipScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.Hip, Core.Width, Core.Height); // Adjust as per actual mapping
            var rootScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.Root, Core.Width, Core.Height);
            var rightCalfScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightCalf, Core.Width, Core.Height);
            var leftCalfScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.LeftCalf, Core.Width, Core.Height);
            var rightFootScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightFoot, Core.Width, Core.Height);
            var leftFootScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.LeftFoot, Core.Width, Core.Height);
            var rightWristScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightWrist, Core.Width, Core.Height);
            var leftHandScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.LeftHand, Core.Width, Core.Height);
            var leftShoulderScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.LeftShoulder, Core.Width, Core.Height);
            var rightShoulderScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightShoulder, Core.Width, Core.Height);
            var rightWristJointScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightWristJoint, Core.Width, Core.Height);
            var leftWristJointScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.LeftWristJoint, Core.Width, Core.Height);
            var leftElbowScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.LeftElbow, Core.Width, Core.Height);
            var rightElbowScreenPos = W2S.WorldToScreen(Core.CameraMatrix, entity.RightElbow, Core.Width, Core.Height); // Adjust if needed

            // Draw skeleton lines


            DrawLine(drawList, spineScreenPos, rightShoulderScreenPos, lineColor); // Spine to Right Shoulder
            DrawLine(drawList, spineScreenPos, hipScreenPos, lineColor);// Spine to hip


            DrawLine(drawList, spineScreenPos, leftShoulderScreenPos, lineColor); // Spine to Left Shoulder
            DrawLine(drawList, leftShoulderScreenPos, rightElbowScreenPos, lineColor); // Left Shoulder to Left Elbow
            DrawLine(drawList, leftElbowScreenPos, rightWristJointScreenPos, lineColor); // Left Elbow to Left Wrist Joint
            // Left Wrist Joint to Left Wrist

            DrawLine(drawList, rightShoulderScreenPos, leftElbowScreenPos, lineColor); // Right Shoulder to Left Elbow
                                                                                       //  DrawLine(drawList, rightElbowScreenPos, leftWristJointScreenPos, lineColor); // Right Elbow to Left Wrist Joint
                                                                                       // Right Wrist Joint to Left Wrist

            DrawLine(drawList, hipScreenPos, rightFootScreenPos, lineColor);// Hip to Right Calf
            DrawLine(drawList, hipScreenPos, leftFootScreenPos, lineColor);// Hip to Left Calf


            // Draw a small circle around the head
            float distance = entity.Distance; // Assume entity.Distance is the distance to the player in game units

            // Calculate the circle radius based on distance (e.g., closer = larger, farther = smaller)
            float baseRadius = 50.0f; // Adjust this base value as needed
            float circleRadius = baseRadius / distance;

            // Draw the circle on the head if the head is visible on screen
            if (headScreenPos.X > 0 && headScreenPos.Y > 0)
            {
                drawList.AddCircle(headScreenPos, circleRadius, circleColor, 30); // 30 segments for the circle
            }

            // Add additional code here to draw the rest of the skeleton using the updated bone positions
        }

        private void DrawLine(ImDrawListPtr drawList, Vector2 startPos, Vector2 endPos, uint color)
        {
            if (startPos.X > 0 && startPos.Y > 0 && endPos.X > 0 && endPos.Y > 0)
            {
                drawList.AddLine(startPos, endPos, color, 1.5f); // Adjust thickness as needed
            }
        }

        public void DrawSmoothCircle(float radius, uint color, float thickness, int segments = 64)
        {
            var vList = ImGui.GetForegroundDrawList();
            var io = ImGui.GetIO();
            float centerX = io.DisplaySize.X / 2;
            float centerY = io.DisplaySize.Y / 2;

            vList.AddCircle(new Vector2(centerX, centerY), radius, color, segments, thickness);
        }



        // Helper function for linear interpolation of colors



        public void DrawHealthBarK(short health, short maxHealth, float X, float Y, float height, float width)
        {
            var vList = ImGui.GetForegroundDrawList();

            // Prevent division by zero and ensure healthPercentage is between 0 and 1
            if (maxHealth <= 0) maxHealth = 100; // Fallback to a default max health
            float healthPercentage = Math.Clamp((float)health / maxHealth, 0f, 1f);
            float healthWidth = width * healthPercentage;

            // Determine the color based on health percentage
            Color healthColor;


            if (healthPercentage < 0.3f)
            {
                healthColor = Color.FromArgb((int)(1f * 255), 255, 0, 0); // Red for health < 20%
            }
            else if (healthPercentage < 0.8f)
            {
                healthColor = Color.FromArgb((int)(1f * 255), 255, 0, 0); // Yellow for health < 70%
            }
            else
            {
                healthColor = Color.FromArgb((int)(1f * 255), 255, 0, 0); // Green for health >= 70%
            }

            // Draw the full health bar background (unfilled part)
            vList.AddRectFilled(new Vector2(X, Y - height), new Vector2(X + width, Y), ColorToUint32(Color.FromArgb((int)(1f * 255), 99, 0, 0))); // Background for health bar

            // Draw the health portion representing current health
            vList.AddRectFilled(new Vector2(X, Y - height), new Vector2(X + healthWidth, Y), ColorToUint32(healthColor)); // Health portion

            // Draw the black outline around the health bar
            vList.AddRect(new Vector2(X, Y - height), new Vector2(X + width, Y), ColorToUint32(Color.Black), 1f); // Black outline
        }


        public void DrawHealthBar(short health, short maxHealth, float X, float Y, float height, float width)
        {
            var vList = ImGui.GetForegroundDrawList();

            // Prevent division by zero and ensure healthPercentage is between 0 and 1
            if (maxHealth <= 0) maxHealth = 100; // Fallback to a default max health
            float healthPercentage = Math.Clamp((float)health / maxHealth, 0f, 1f);
            float healthWidth = width * healthPercentage;

            // Determine the color based on health percentage
            Color healthColor;


            if (healthPercentage < 0.3f)
            {
                healthColor = Color.FromArgb((int)(1f * 255), 255, 0, 0); // Red for health < 20%
            }
            else if (healthPercentage < 0.8f)
            {
                healthColor = Color.FromArgb((int)(1f * 255), 255, 255, 0); // Yellow for health < 70%
            }
            else
            {
                healthColor = Color.FromArgb((int)(1f * 255), 86, 255, 43); // Green for health >= 70%
            }

            // Draw the full health bar background (unfilled part)
            vList.AddRectFilled(new Vector2(X, Y - height), new Vector2(X + width, Y), ColorToUint32(Color.FromArgb((int)(1f * 255), 99, 0, 0))); // Background for health bar

            // Draw the health portion representing current health
            vList.AddRectFilled(new Vector2(X, Y - height), new Vector2(X + healthWidth, Y), ColorToUint32(healthColor)); // Health portion

            // Draw the black outline around the health bar
            vList.AddRect(new Vector2(X, Y - height), new Vector2(X + width, Y), ColorToUint32(Color.Black), 1f); // Black outline
        }




        private Color LerpColor(Color start, Color end, float amount)
        {
            return Color.FromArgb(
                (int)(start.A + (end.A - start.A) * amount),
                (int)(start.R + (end.R - start.R) * amount),
                (int)(start.G + (end.G - start.G) * amount),
                (int)(start.B + (end.B - start.B) * amount)
            );
        }











        static uint ColorToUint32(Color color)
        {

            return ImGui.ColorConvertFloat4ToU32(new Vector4(
            (float)(color.R / 255.0),
                (float)(color.G / 255.0),
                (float)(color.B / 255.0),
                (float)(color.A / 255.0)));
            return ((uint)color.A << 24) | ((uint)color.B << 16) | ((uint)color.G << 8) | color.R;
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);


        const uint WDA_NONE = 0x00000000;
        const uint WDA_MONITOR = 0x00000001;
        const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
        void CreateHandle()
        {

            RECT rect;
            GetWindowRect(Core.Handle, out rect);
            int x = rect.Left;
            int y = rect.Top;
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;
            ImGui.SetWindowSize(new Vector2((float)width, (float)height));
            ImGui.SetWindowPos(new Vector2((float)x, (float)y));
            Size = new Size(width, height);
            Position = new Point(x, y);
            Core.Width = width;
            Core.Height = height;
            if (Config.StreamMode)
            {
                SetWindowDisplayAffinity(hWnd, WDA_EXCLUDEFROMCAPTURE);
            }
            else
            {
                SetWindowDisplayAffinity(hWnd, WDA_NONE);
            }

        }
    }
}