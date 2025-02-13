using System.Collections.Generic;
using ArcCreate.Utility.Parser;

namespace ArcCreate.ChartFormat
{
    public class RawTimingGroup
    {
        // Might be problematic when multiple chart formats is introduced, as this class only serialize / deserialize to aff.
        // Don't wnat to think about it now though.
        public RawTimingGroup()
        {
            NoInput = false;
            NoClip = false;
            FadingHolds = false;
            NoHeightIndicator = false;
            NoShadow = false;
            NoArcCap = false;
            NoHead = false;
            AngleX = 0;
            AngleY = 0;
            ArcResolution = 1;
            Side = SideOverride.None;
        }

        public RawTimingGroup(string def, int line = 0)
        {
            if (def == "")
            {
                return;
            }

            string[] split = def.Split(',');
            foreach (string optRaw in split)
            {
                string opt = optRaw.Trim().ToLower();
                if (opt.Contains("="))
                {
                    string[] tokens = opt.Split('=');
                    string type = tokens[0];
                    string value = tokens[1];

                    bool valid;
                    float val;
                    switch (type)
                    {
                        case "name":
                            Name = value.Trim('"');
                            break;
                        case "anglex":
                            valid = Evaluator.TryFloat(value, out val);
                            AngleX = valid ? val : 0;
                            break;
                        case "angley":
                            valid = Evaluator.TryFloat(value, out val);
                            AngleY = valid ? val : 0;
                            break;
                        case "arcresolution":
                            valid = Evaluator.TryFloat(value, out val);
                            val = UnityEngine.Mathf.Clamp(val, 0, 10);
                            ArcResolution = valid ? val : 1;
                            break;
                        default:
                            throw new ChartFormatException(
                                RawEventType.TimingGroup,
                                opt,
                                File,
                                line,
                                I18n.S("Format.Exception.TimingGroupPropertiesInvalid"));
                    }
                }
                else
                {
                    switch (opt)
                    {
                        case "noinput":
                            NoInput = true;
                            break;
                        case "noclip":
                            NoClip = true;
                            break;
                        case "noheightindicator":
                            NoHeightIndicator = true;
                            break;
                        case "nohead":
                            NoHead = true;
                            break;
                        case "noshadow":
                            NoShadow = true;
                            break;
                        case "noarccap":
                            NoArcCap = true;
                            break;
                        case "light":
                            Side = SideOverride.Light;
                            break;
                        case "conflict":
                            Side = SideOverride.Conflict;
                            break;
                        case "fadingholds":
                            FadingHolds = true;
                            break;
                        default:
                            throw new ChartFormatException(
                                RawEventType.TimingGroup,
                                opt,
                                File,
                                line,
                                I18n.S("Format.Exception.TimingGroupPropertiesInvalid"));
                    }
                }
            }
        }

        public string Name { get; set; } = null;

        public bool NoInput { get; set; } = false;

        public bool NoClip { get; set; } = false;

        public bool NoHeightIndicator { get; set; } = false;

        public bool NoShadow { get; set; } = false;

        public bool NoHead { get; set; } = false;

        public bool NoArcCap { get; set; } = false;

        public bool FadingHolds { get; set; } = false;

        public float ArcResolution { get; set; } = 1;

        public float AngleX { get; set; } = 0;

        public float AngleY { get; set; } = 0;

        public SideOverride Side { get; set; }

        public string File { get; set; } = "";

        public bool Editable { get; set; } = true;

        public override string ToString()
        {
            var opts = GetPropertyStrings(true);
            return string.Join(",", opts);
        }

        public string ToStringWithoutName()
        {
            var opts = GetPropertyStrings(false);
            return string.Join(",", opts);
        }

        private List<string> GetPropertyStrings(bool withName)
        {
            List<string> opts = new List<string>();
            if (withName && !string.IsNullOrEmpty(Name))
            {
                opts.Add($"name=\"{Name}\"");
            }

            if (NoInput)
            {
                opts.Add("noinput");
            }

            if (NoClip)
            {
                opts.Add("noclip");
            }

            if (NoHeightIndicator)
            {
                opts.Add("noheightindicator");
            }

            if (NoHead)
            {
                opts.Add("nohead");
            }

            if (NoShadow)
            {
                opts.Add("noshadow");
            }

            if (NoArcCap)
            {
                opts.Add("noarccap");
            }

            if (FadingHolds)
            {
                opts.Add("fadingholds");
            }

            if (AngleX != 0)
            {
                opts.Add($"anglex={AngleX:f2}");
            }

            if (AngleY != 0)
            {
                opts.Add($"angley={AngleY:f2}");
            }

            if (ArcResolution != 1)
            {
                opts.Add($"arcresolution={ArcResolution:f1}");
            }

            if (Side != SideOverride.None)
            {
                opts.Add(Side == SideOverride.Light ? "light" : "conflict");
            }

            return opts;
        }
    }
}