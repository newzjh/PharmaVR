﻿/*
 * Copyright 2012 ZXing.Net authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#if UNITY_5_3_OR_NEWER
#define UNITY
#endif

using System;
#if UNITY
using UnityEngine;
using Color = UnityEngine.Color32;
#elif MONOANDROID
using Android.Graphics;
#elif (PORTABLE || NETSTANDARD)
#elif (NET45 || NET40 || NET35 || NET20 || WindowsCE)
using System.Drawing;
#elif NETFX_CORE
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

using ZXing.Common;
using ZXing.OneD;

namespace ZXing.Rendering
{
   /// <summary>
   /// Renders a <see cref="BitMatrix" /> to an byte array with pixel data (4 byte per pixel, BGRA)
   /// </summary>
   public sealed class PixelDataRenderer : IBarcodeRenderer<PixelData>
   {
#if (PORTABLE || NETSTANDARD) && !UNITY
      public struct Color
      {
         public static Color Black = new Color(0);
         public static Color White = new Color(0x00FFFFFF);

         public byte A;
         public byte R;
         public byte G;
         public byte B;

         public Color(int color)
         {
            A = (byte)((color & 0xFF000000) >> 24);
            R = (byte)((color & 0x00FF0000) >> 16);
            G = (byte)((color & 0x0000FF00) >> 8);
            B = (byte)((color & 0x000000FF));
         }
      }
#endif
      /// <summary>
      /// Gets or sets the foreground color.
      /// </summary>
      /// <value>
      /// The foreground color.
      /// </value>
      [System.CLSCompliant(false)]
      public Color Foreground { get; set; }
      /// <summary>
      /// Gets or sets the background color.
      /// </summary>
      /// <value>
      /// The background color.
      /// </value>
      [System.CLSCompliant(false)]
      public Color Background { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="PixelDataRenderer"/> class.
      /// </summary>
      public PixelDataRenderer()
      {
#if UNITY
         Foreground = UnityEngine.Color.black;
         Background = UnityEngine.Color.white;
#elif (NET45 || NET40 || NET35 || NET20 || WindowsCE || PORTABLE || NETSTANDARD || MONOANDROID)
         Foreground = Color.Black;
         Background = Color.White;
#else
         Foreground = Colors.Black;
         Background = Colors.White;
#endif
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <returns></returns>
      public PixelData Render(BitMatrix matrix, BarcodeFormat format, string content)
      {
         return Render(matrix, format, content, null);
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <param name="options">The options.</param>
      /// <returns></returns>
      public PixelData Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         int width = matrix.Width;
         int heigth = matrix.Height;
         bool outputContent = (options == null || !options.PureBarcode) &&
                              !String.IsNullOrEmpty(content) && (format == BarcodeFormat.CODE_39 ||
                                                                 format == BarcodeFormat.CODE_128 ||
                                                                 format == BarcodeFormat.EAN_13 ||
                                                                 format == BarcodeFormat.EAN_8 ||
                                                                 format == BarcodeFormat.CODABAR ||
                                                                 format == BarcodeFormat.ITF ||
                                                                 format == BarcodeFormat.UPC_A ||
                                                                 format == BarcodeFormat.MSI ||
                                                                 format == BarcodeFormat.PLESSEY);
         int emptyArea = outputContent ? 16 : 0;
         int pixelsize = 1;

         if (options != null)
         {
            if (options.Width > width)
            {
               width = options.Width;
            }
            if (options.Height > heigth)
            {
               heigth = options.Height;
            }
            // calculating the scaling factor
            pixelsize = width / matrix.Width;
            if (pixelsize > heigth / matrix.Height)
            {
               pixelsize = heigth / matrix.Height;
            }
         }

         var pixels = new byte[width * heigth * 4];
         var index = 0;

         for (int y = 0; y < matrix.Height - emptyArea; y++)
         {
            for (var pixelsizeHeight = 0; pixelsizeHeight < pixelsize; pixelsizeHeight++)
            {
               for (var x = 0; x < matrix.Width; x++)
               {
                  var color = matrix[x, y] ? Foreground : Background;
                  for (var pixelsizeWidth = 0; pixelsizeWidth < pixelsize; pixelsizeWidth++)
                  {
#if UNITY
                     pixels[index++] = color.b;
                     pixels[index++] = color.g;
                     pixels[index++] = color.r;
                     pixels[index++] = color.a;
#else
                     pixels[index++] = color.B;
                     pixels[index++] = color.G;
                     pixels[index++] = color.R;
                     pixels[index++] = color.A;
#endif
                  }
               }
               for (var x = pixelsize * matrix.Width; x < width; x++)
               {
#if UNITY
                  pixels[index++] = Background.b;
                  pixels[index++] = Background.g;
                  pixels[index++] = Background.r;
                  pixels[index++] = Background.a;
#else
                  pixels[index++] = Background.B;
                  pixels[index++] = Background.G;
                  pixels[index++] = Background.R;
                  pixels[index++] = Background.A;
#endif
               }
            }
         }
         for (int y = matrix.Height * pixelsize - emptyArea; y < heigth; y++)
         {
            for (var x = 0; x < width; x++)
            {
#if UNITY
               pixels[index++] = Background.b;
               pixels[index++] = Background.g;
               pixels[index++] = Background.r;
               pixels[index++] = Background.a;
#else
               pixels[index++] = Background.B;
               pixels[index++] = Background.G;
               pixels[index++] = Background.R;
               pixels[index++] = Background.A;
#endif
            }
         }

         return new PixelData(width, heigth, pixels);
      }
   }
}
