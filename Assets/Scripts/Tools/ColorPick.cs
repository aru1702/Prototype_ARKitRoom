using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * src:
 * - https://jsfiddle.net/wq84510v/
 * - https://stackoverflow.com/questions/10848990/rgb-values-to-0-to-1-scale
 * - https://en.wikipedia.org/wiki/HSL_and_HSV
 * - https://stackoverflow.com/questions/17525215/calculate-color-values-from-green-to-red
 */

public class ColorPick 
{
    private static float LOWEST_VALUE { get; set; }
    private static float HIGHEST_VALUE { get; set; }

    public struct RGBColor
    {
        public RGBColor(float R, float G, float B)
        {
            r = R;
            g = G;
            b = B;
        }

        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }

        public override string ToString() => $"({r}, {g}, {b})";
    }

    public static Color GetColor(float value, float lo, float hi, float alpha = 1.0f)
    {
        LOWEST_VALUE = lo; HIGHEST_VALUE = hi;

        float hue = Normalized(value) * 1.2f / 3600f;
        RGBColor rgb = HSLtoRGBColor(hue, 1.0f, 0.5f);

        return new Color(RGB_C(rgb.r), RGB_C(rgb.g), RGB_C(rgb.b), alpha);
    }

    public static Color GetColor(float value, List<float> allValue, float alpha = 1.0f)
    {
        LOWEST_VALUE = allValue.Min(); HIGHEST_VALUE = allValue.Max();

        float hue = Normalized(value) * 1.2f / 3600f;
        RGBColor rgb = HSLtoRGBColor(hue, 1.0f, 0.5f);

        return new Color(RGB_C(rgb.r), RGB_C(rgb.g), RGB_C(rgb.b), alpha);
    }

    public static List<Color> GetColors(List<float> allValue, float alpha = 1.0f)
    {
        LOWEST_VALUE = allValue.Min(); HIGHEST_VALUE = allValue.Max();
        List<Color> colorList = new List<Color>();

        foreach (var value in allValue)
        {
            float hue = Normalized(value) * 1.2f / 3600f;
            RGBColor rgb = HSLtoRGBColor(hue, 1.0f, 0.5f);

            colorList.Add(new Color(RGB_C(rgb.r), RGB_C(rgb.g), RGB_C(rgb.b), alpha));
        }

        return colorList;
    }

    private static RGBColor HSLtoRGBColor(float h, float s, float l)
    {
        float r, g, b;

        if (s == 0)
        {
            r = g = b = l;
        }
        else
        {
            float q; if (l < 0.5f) { q = l * (1 + s); } else { q = l + s - l * s; }
            float p = 2 * l - q;
            r = HUEtoRGB(p, q, h + 0.333f);
            g = HUEtoRGB(p, q, h);
            b = HUEtoRGB(p, q, h - 0.333f);
        }

        return new RGBColor(Mathf.Floor(r * 255), Mathf.Floor(g * 255), Mathf.Floor(b * 255));
    }

    private static float HUEtoRGB(float p, float q, float t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 0.167f) return p + (q - p) * 6 * t;
        if (t < 0.5f) return q;
        if (t < 0.667f) return p + (q - p) * (0.667f - t) * 6;
        return p;
    }

    // normalized is returning value between 0 ~ 1000, not 0 ~ 1
    private static float Normalized(float value)
    {
        return (value - LOWEST_VALUE) * 1000 / (HIGHEST_VALUE - LOWEST_VALUE);
    }

    private static float RGB_C(float value)
    {
        if (value != 0) { return value / 255; } else { return 0; }
    }
}
