﻿using GoogleMobileAds.Api;
using IngameDebugConsole;
using Newtonsoft.Json.Linq;
using Singular;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static GoogleMobileAds.Api.AdValue;

public enum TypeSceneCurrent
{
    BeginScene,
    HomeScene,
    GameplayScene,
    BuildPixelArtScene,
    DIYScene
}
public static class ActionHelper
{
    public static bool IsStartGame = true;
    public static TypeSceneCurrent GetSceneCurrent()
    {
        if (HomeUIManager.instance != null)
            return TypeSceneCurrent.HomeScene;
        if (GameplayUIManager.instance != null)
            return TypeSceneCurrent.GameplayScene;
        return TypeSceneCurrent.BeginScene;
    }

    #region [ Safe Invoke ]
    public static void SafeInvoke(this Action action)
    {
        if (action != null)
        {
            action();
        }
    }
    public static void SafeInvoke<T>(this Action<T> action, T par)
    {
        if (action != null)
        {
            action(par);
        }
    }
    public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 par1, T2 par2)
    {
        if (action != null)
        {
            action(par1, par2);
        }
    }
    public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 par1, T2 par2, T3 par3)
    {
        if (action != null)
        {
            action(par1, par2, par3);
        }
    }

    public static void SafeInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 par1, T2 par2, T3 par3, T4 par4)
    {
        if (action != null)
        {
            action(par1, par2, par3, par4);
        }
    }

    public static Texture2D Rotate(Texture2D originalTexture, bool clockwise)
    {
        Color32[] pixels = originalTexture.GetPixels32();
        Color32[] colors = new Color32[pixels.Length];
        int width = originalTexture.width;
        int height = originalTexture.height;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int destIndex = (j + 1) * height - i - 1;
                int sourceIndex = (!clockwise) ? (i * width + j) : (pixels.Length - 1 - (i * width + j));
                colors[destIndex] = pixels[sourceIndex];
            }
        }
        Texture2D texture2D = new Texture2D(height, width);
        texture2D.SetPixels32(colors);
        texture2D.Apply();
        return texture2D;
    }
    #endregion

    #region [ Funct Helper ]
    public static IEnumerator StartAction(UnityAction action, float delay, bool unscaleTime = false)
    {
        if (unscaleTime)
        {
            yield return new WaitForSecondsRealtime(delay);
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }
        action?.Invoke();
    }
    /// <summary>
    /// Clear ALL object child of object parent
    /// </summary>
    /// <param name="transform"></param>
    public static void Clear(this Transform transform)
    {
        while (transform.childCount > 0)
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
    }
    /// <summary>
    /// set Vibration
    /// </summary>
    public static void SetVibration(long milliseconds)
    {
        if (!IsEditor())
        {
            // call vibration
            if (VariableSystem.Vibrate)
            {
                Debug.Log("SetVibration " + milliseconds);
                long[] pattern = { 0, milliseconds, 0, 0, 0 };
                Vibration.VibrateAndroid(pattern, -1);
                // Vibration.VibrateAndroid(500);
                //Vibration.VibrateNope();
            }
        }
    }
    /// <summary>
    /// Cancel Vibration
    /// </summary>
    public static void CancelVibration()
    {
        if (!IsEditor())
        {
            // cancel vibration
            if (VariableSystem.Vibrate)
            {
                Debug.Log("CancelVibration");
                Vibration.CancelAndroid();
            }
        }
    }
    #region TextureColorsReducer

    private static int s_width;

    private static int s_height;

    private static int s_colorsCount;

    public static Texture2D Process(Texture2D sourceTex, int colorsCount)
    {
        s_colorsCount = colorsCount;
        s_width = sourceTex.width;
        s_height = sourceTex.height;
        byte[] bytes = ConvertColorsToBytes(sourceTex.GetPixels32());
        Color32[] pixels = AnalyzeAndChangePixels(bytes);
        Texture2D texture2D = new Texture2D(s_width, s_height, sourceTex.format, mipChain: false);
        texture2D.filterMode = sourceTex.filterMode;
        Texture2D texture2D2 = texture2D;
        texture2D2.SetPixels32(pixels);
        texture2D2.Apply();
        return texture2D2;
    }

    public static byte[] ConvertColorsToBytes(Color32[] data)
    {
        byte[] array = new byte[3 * s_width * s_height];
        int num = 0;
        for (int num2 = s_height - 1; num2 >= 0; num2--)
        {
            for (int i = 0; i < s_width; i++)
            {
                Color32 color = data[num2 * s_width + i];
                array[num] = color.r;
                num++;
                array[num] = color.g;
                num++;
                array[num] = color.b;
                num++;
            }
        }

        return array;
    }

    public static Color32[] AnalyzeAndChangePixels(byte[] bytes)
    {
        int num = bytes.Length;
        int num2 = num / 3;
        Color32[] array = new Color32[num2];
        NeuQuant neuQuant = new NeuQuant(bytes, num, 10, s_colorsCount);
        byte[] array2 = neuQuant.Process();
        int num3 = 0;
        for (int num4 = s_height - 1; num4 >= 0; num4--)
        {
            for (int i = 0; i < s_width; i++)
            {
                int num5 = neuQuant.Map(bytes[num3++] & 0xFF, bytes[num3++] & 0xFF, bytes[num3++] & 0xFF);
                ref Color32 reference = ref array[num4 * s_width + i];
                reference = new Color32(array2[num5 * 3], array2[num5 * 3 + 1], array2[num5 * 3 + 2], byte.MaxValue);
            }
        }

        return array;
    }

    #endregion
    public static string ClassifyTypeBooster(TypeBooster type)
    {
        string _type = "Number";
        switch (type)
        {
            case TypeBooster.Number:
                _type = "Paint Brush";
                break;
            case TypeBooster.Bomb:
                _type = "Paint Bucket";
                break;
            case TypeBooster.Find:
                _type = "Find Number";
                break;
        }
        return _type;
    }
    public static void CamScaleTexture(Texture2D texture, int height, float ratio, float quality)
    {
        TextureScale.Point(texture, (int)((float)height * ratio * quality), (int)((float)height * quality));
    }
    public static List<int> SplitNumberIntoDigits(int number)
    {
        List<int> digits = new List<int>();

        while (number > 0)
        {
            digits.Add(number % 10);
            number /= 10;
        }

        digits.Reverse();
        return digits;
    }
    public static List<int> ConfigListIntFromString(string str)
    {
        var list = new List<int>();
        string s = "";
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == ',')
            {
                var val = int.Parse(s);
                list.Add(val);
                s = "";
            }
            else
            {
                s += str[i];
            }
        }
        return list;
    }
    public static List<TypeTopic> ConfigListTypeTopicFromString(string str)
    {
        var list = ConfigListIntFromString(str);
        var lst = new List<TypeTopic>();
        foreach (var t in list)
            lst.Add((TypeTopic)t);

        return lst;
    }
    public static Texture2D ToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
    public static Texture2D SpriteToTexture(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }
    public static Sprite Texture2DToSprite(Texture2D texture)
    {
        // Tạo sprite từ Texture2D
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f); // Center pivot

        return Sprite.Create(texture, rect, pivot);
    }
    public static string ToRGBHex(Color c)
    {
        return ColorUtility.ToHtmlStringRGBA(c);
    }
    public static Color HexToColor(string hex)
    {
        Color newCol;

        if (ColorUtility.TryParseHtmlString(hex, out newCol))
            return newCol;

        return Color.black;
    }
    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }
    public static string TextureToString(Texture2D texture)
    {
        byte[] textureBytes = texture.EncodeToPNG();
        return System.Convert.ToBase64String(textureBytes);
    }

    public static Texture2D StringToTexture(string base64String)
    {
        byte[] textureBytes = System.Convert.FromBase64String(base64String);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(textureBytes);
        return texture;
    }
    //======================================================================================================================
    public static DateTime ConfigTimeLateMonth(DateTime _date)
    {
        if (_date.Month % 2 == 0) // 2 4 6 8 10 12
        {
            if (_date.Month == 2)
            {
                if (DateTime.IsLeapYear(_date.Year))
                {
                    if (_date.Day + 1 > 29)
                    {
                        DateTime dateTime = new DateTime(_date.Year, _date.Month + 1, 1);
                        return dateTime;
                    }
                }
                else
                {

                    if (_date.Day + 1 > 28)
                    {
                        DateTime dateTime = new DateTime(_date.Year, _date.Month + 1, 1);
                        return dateTime;
                    }
                }
            }

            if (_date.Month < 7) // 30 day
            {
                if (_date.Day + 1 > 30)
                {
                    DateTime dateTime = new DateTime(_date.Year, _date.Month + 1, 1);
                    return dateTime;
                }
            }
            else // 31 day
            {
                if (_date.Month == 12)
                {
                    if (_date.Day + 1 > 31)
                    {
                        DateTime dateTime = new DateTime(_date.Year + 1, 1, 1);
                        return dateTime;
                    }
                }
                else
                {
                    if (_date.Day + 1 > 31)
                    {
                        DateTime dateTime = new DateTime(_date.Year, _date.Month + 1, 1);
                        return dateTime;
                    }
                }
            }
        }
        else // 1 3 5 7 9 11
        {
            if (_date.Month <= 7) // 31 day
            {
                if (_date.Day + 1 > 31)
                {
                    DateTime dateTime = new DateTime(_date.Year, _date.Month + 1, 1);
                    return dateTime;
                }
            }
            else // 30 day
            {
                if (_date.Day + 1 > 30)
                {
                    DateTime dateTime = new DateTime(_date.Year + 1, 1, 1);
                    return dateTime;
                }
            }
        }
        return new DateTime(_date.Year, _date.Month, _date.Day + 1);
    }

    /// <summary>
    /// fomat time Example: 120 sec to 02 min: 00 sec
    /// </summary>
    /// <param name="sec"></param>
    /// <returns>return time fomat min:sec</returns>
    public static string GetStringTime(int sec)
    {
        int _min = sec / 60;
        int _sec = sec % 60;
        return _min + ":" + _sec;
    }
    public static bool ContainsOnlyAlphaNumericCharacters(this string inputString)
    {
        var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
        return regexItem.IsMatch(inputString);
    }

    /// <summary>
    /// format coin by characters
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToShortFormat(float value)
    {
        int num = 0;
        while (value >= 1000f)
        {
            value *= 0.001f;
            num++;
        }
        string arg = string.Empty;
        switch (num)
        {
            case 1:
                arg = "K";
                break;
            case 2:
                arg = "M";
                break;
            case 3:
                arg = "B";
                break;
            case 4:
                arg = "T";
                break;
            case 5:
                arg = "aa";
                break;
            case 6:
                arg = "bb";
                break;
            case 7:
                arg = "cc";
                break;
            case 8:
                arg = "dd";
                break;
            case 9:
                arg = "ee";
                break;
            case 10:
                arg = "ff";
                break;
            case 11:
                arg = "gg";
                break;
            case 12:
                arg = "hh";
                break;
            case 13:
                arg = "ii";
                break;
            case 14:
                arg = "jj";
                break;
            case 15:
                arg = "kk";
                break;
            case 16:
                arg = "ll";
                break;
            case 17:
                arg = "mm";
                break;
            case 18:
                arg = "nn";
                break;
            case 19:
                arg = "oo";
                break;
            case 20:
                arg = "pp";
                break;
            case 21:
                arg = "qq";
                break;
            case 22:
                arg = "rr";
                break;
            case 23:
                arg = "ss";
                break;
            case 24:
                arg = "tt";
                break;
            case 25:
                arg = "uu";
                break;
            case 26:
                arg = "vv";
                break;
            case 27:
                arg = "ww";
                break;
            case 28:
                arg = "xx";
                break;
            case 29:
                arg = "yy";
                break;
            case 30:
                arg = "zz";
                break;
        }
        return string.Format((num != 0) ? "{0:0.##}{1}" : "{0:F0}", value, arg);
    }
    /// <summary>
    /// Loading looks like this: . .. ... 
    /// </summary>
    /// <returns></returns>
    public static IEnumerator IE_TextLoading(Text txtLoading, string content, float timeVisual)
    {
        int i = 0;
        while (true)
        {
            switch (i)
            {
                case 0:
                    txtLoading.text = content + " . ";
                    break;
                case 1:
                    txtLoading.text = content + " .. ";
                    break;
                case 2:
                    txtLoading.text = content + " ... ";
                    i = -1;
                    break;
            }
            i++;
            yield return new WaitForSeconds(timeVisual);
        }
    }

    /// <summary>
    /// Config Date
    /// </summary>
    /// <param name="date">valeu fig</param>
    /// <returns>if config success return != -1</returns>
    public static int ConfigDate(int date)
    {
        int _d = DateTime.Now.DayOfYear;

        if (date <= _d)
            return _d;

        if (date > _d)
            if (_d == 1 && date != (_d + 1))
                return 1;

        return -1;
    }
    public static string ConfigString(string str)
    {
        char[] arr = str.ToCharArray();
        string s = "0123456789";
        var regexItem = new Regex("^[a-zA-Z -]*$");

        for (int i = 0; i < arr.Length; i++)
            if (regexItem.IsMatch(arr[i].ToString()))
                arr[i] = s[UnityEngine.Random.Range(0, s.Length)];

        return new string(arr);
    }
    public static string RandomNamePlayer(string idPlayer)
    {
        string _result = "";

        string _idPlayer = idPlayer;

        char[] arr = _idPlayer.ToCharArray();
        Array.Reverse(arr);
        _idPlayer = new string(arr);

        int randomChar = UnityEngine.Random.Range(10, 14);

        _idPlayer = _idPlayer.Substring(0, randomChar);
        arr = _idPlayer.ToCharArray();
        Array.Reverse(arr);

        //string s = "qwertyuioplkjhgfdsazxcvbnm";
        string s = "0123456789";
        var regexItem = new Regex("^[a-zA-Z -]*$");

        for (int i = 0; i < arr.Length; i++)
        {
            if (regexItem.IsMatch(arr[i].ToString()))
            {
                arr[i] = s[UnityEngine.Random.Range(0, s.Length)];
            }
        }
        _result = new string(arr);
        return _result;
    }

    // --------------- Random Fake Name Player -------------------
    public static string RandomFakeName()
    {
        #region MaleFirstNames

        string TEMP_male_firstNames_data =
            "Jacob Ethan Michael Jayden William Alexander Noah Daniel Aiden Anthony Joshua Mason Christopher Andrew " +
            "David Matthew Logan Elijah James Joseph Gabriel Benjamin Ryan Samuel Jackson John Nathan Jonathan Christian Liam Dylan Landon " +
            "Caleb Tyler Lucas Evan Gavin Nicholas Isaac Brayden Luke Angel Brandon Jack Isaiah Jordan Owen Carter Connor Justin Jose Jeremiah " +
            "Julian Robert Aaron Adrian Wyatt Kevin Hunter Cameron Zachary Thomas Charles Austin Eli Chase Henry Sebastian Jason Levi Xavier " +
            "Ian Colton Dominic Juan Cooper Josiah Luis Ayden Carson Adam Nathaniel Brody Tristan Diego Parker Blake Oliver Cole Carlos Jaden " +
            "Jesus Alex Aidan Eric Hayden Bryan Max Jaxon Brian Bentley Alejandro Sean Nolan Riley Kaden Kyle Micah Vincent Antonio Colin " +
            "Bryce Miguel Giovanni Timothy Jake Kaleb Steven Caden Bryson Damian Grayson Kayden Jesse Brady Ashton Richard Victor Patrick " +
            "Marcus Preston Joel Santiago Maxwell Ryder Edward Miles Hudson Asher Devin Elias Jeremy Ivan Jonah Easton Jace Oscar Collin " +
            "Peyton Leonardo Cayden Gage Eduardo Emmanuel Grant Alan Conner Cody Wesley Kenneth Mark Nicolas Malachi George Seth Kaiden " +
            "Trevor Jorge Derek Jude Braxton Jaxson Sawyer Jaiden Omar Tanner Travis Paul Camden Maddox Andres Cristian Rylan Josue Roman " +
            "Bradley Axel Fernando Garrett Javier Damien Peter Leo Abraham Ricardo Francisco Lincoln Erick Drake Shane Cesar Stephen Jaylen " +
            "Tucker Kai Landen Braden Mario Edwin Avery Manuel Trenton Ezekiel Kingston Calvin Edgar Johnathan Donovan Alexis Israel Mateo " +
            "Silas Jeffrey Weston Raymond Hector Spencer Andre Brendan Zion Griffin Lukas Maximus Harrison Andy Braylon Tyson Shawn Sergio " +
            "Zane Emiliano Jared Ezra Charlie Keegan Chance Drew Troy Greyson Corbin Simon Clayton Myles Xander Dante Erik Rafael Martin " +
            "Dominick Dalton Cash Skyler Theodore Marco Caiden Johnny Ty Gregory Kyler Roberto Brennan Luca Emmett Kameron Declan Quinn " +
            "Jameson Amir Bennett Colby Pedro Emanuel Malik Graham Dean Jasper Everett Aden Dawson Angelo Reid Abel Dakota Zander Paxton " +
            "Ruben Judah Jayce Jakob Finn Elliot Frank Lane Fabian Dillon Brock Derrick Emilio Joaquin Marcos Ryker Anderson Grady Devon " +
            "Elliott Holden Amari Dallas Corey Danny Cruz Lorenzo Allen Trey Leland Armando Rowan Taylor Cade Colt Felix Adan Jayson Tristen " +
            "Julius Raul Braydon Zayden Julio Nehemiah Darius Ronald Louis Trent Keith Payton Enrique Jax Randy Scott Desmond Gerardo Jett " +
            "Dustin Phillip Beckett Ali Romeo Kellen Cohen Pablo Ismael Jaime Brycen Larry Kellan Keaton Gunner Braylen Brayan Landyn Walter " +
            "Jimmy Marshall Beau Saul Donald Esteban Karson Reed Phoenix Brenden Tony Kade Jamari Jerry Mitchell Colten Arthur Brett Dennis " +
            "Rocco Jalen Tate Chris Quentin Titus Casey Brooks Izaiah Mathew King Philip Zackary Darren Russell Gael Albert Braeden Dane " +
            "Gustavo Kolton Cullen Jay Rodrigo Alberto Leon Alec Damon Arturo Waylon Milo Davis Walker Moises Kobe Curtis Matteo August " +
            "Mauricio Marvin Emerson Maximilian Reece Orlando River Bryant Issac Yahir Uriel Hugo Mohamed Enzo Karter Lance Porter Maurice " +
            "Leonel Zachariah Ricky Joe Johan Nikolas Dexter Jonas Justice Knox Lawrence Salvador Alfredo Gideon Maximiliano Nickolas Talon " +
            "Byron Orion Solomon Braiden Alijah Kristopher Rhys Gary Jacoby Davion Jamarion Pierce Sam Cason Noel Ramon Kason Mekhi Shaun " +
            "Warren Douglas Ernesto Ibrahim Armani Cyrus Quinton Isaias Reese Jaydon Ryland Terry Frederick Chandler Jamison Deandre Dorian " +
            "Khalil Ari Franklin Maverick Amare Muhammad Ronan London Eddie Moses Roger Aldo Nasir Demetrius Adriel Brodie Kelvin Morgan " +
            "Tobias Ahmad Keagan Prince Trace Alvin Giovani Kendrick Malcolm Skylar Conor Camron Abram Jonathon Bruce Noe Quincy Rohan Ahmed " +
            "Nathanael Barrett Remington Kamari Kristian Kieran Finnegan Boston Xzavier Chad Guillermo Uriah Archer Rodney Gunnar Micheal " +
            "Ulises Bobby Aaden Kamden Roy Kane Kasen Julien Ezequiel Lucian Atticus Javon Melvin Jeffery Terrance Nelson Aarav Carl Malakai " +
            "Jadon Triston Harley Jon Kian Alonzo Cory Marc Moshe Gianni Kole Dayton Jermaine Asa Wilson Felipe Kale Terrence Nico Dominik " +
            "Tommy Kendall Cristopher Isiah Finley Tristin Cannon Mohammed Wade Kash Marlon Ariel Madden Rhett Jase Layne Memphis Allan " +
            "Jamal Nash Jessie Joey Reginald Giovanny Lawson Zaiden Ace Korbin Rashad Will Urijah Billy Aron Brennen Branden Leonard Rene " +
            "Kenny Tomas Willie Darian Kody Brendon Aydan Alonso Blaine Arjun Raiden Layton Marquis Sincere Terrell Channing Chace Iker " +
            "Mohammad Jordyn Messiah Omari Santino Sullivan Brent Raphael Deshawn Elisha Harry Luciano Jefferson Jaylin Ray Yandel Aydin " +
            "Craig Tristian Zechariah Bently Francis Toby Tripp Kylan Semaj Alessandro Alexzander Lee Ronnie Gerald Dwayne Jadiel Javion " +
            "Markus Kolby Neil Stanley Makai Davin Teagan Cale Harper Callen Ben Kaeden Clark Jamie Damarion Davian Deacon Jairo Kareem " +
            "Damion Jamir Aidyn Lamar Duncan Matias Rex Jaeden Jasiah Jorden Vicente Aryan Case Tyrone Yusuf Gavyn Lewis Rogelio Zayne " +
            "Giancarlo Osvaldo Rolando Camren Luka Rylee Cedric Jensen Soren Darwin Draven Maxim Ellis Nikolai Bradyn Mathias Zackery Zavier " +
            "Emery Brantley Rudy Trevon Alfonso Beckham Darrell Harold Jerome Daxton Royce Jaylon Rory Rodolfo Tatum Bruno Sterling Gauge Van " +
            "Hamza Ayaan Rayan Zachery Keenan Jagger Heath Jovani Killian Dax Junior Misael Roland Ramiro Vance Alvaro Bode Conrad Eugene " +
            "Augustus Carmelo Adrien Kamron Gilberto Johnathon Kolten Wayne Zain Quintin Steve Tyrell Niko Antoine Hassan Jean Coleman Elian " +
            "Frankie Valentin Adonis Jamar Jaxton Kymani Bronson Clay Freddy Jeramiah Kayson Hank Abdiel Efrain Leandro Yosef Aditya Ean " +
            "Konnor Sage Samir Todd Lyric Deven Derick Jovanni Valentino Demarcus Ishaan Konner Kyson Deangelo Matthias Maximo Sidney " +
            "Benson Dilan Gilbert Kyron Xavi Bo Sylas Fisher Marcel Franco Jaron Alden Agustin Bentlee Malaki Westin Cael Jerimiah Randall " +
            "Blaze Branson Brogan Callum Dominique Justus Krish Rey Marcelo Ronin Odin Camryn Jair Izayah Brice Jabari Mayson Isai Tyree " +
            "Mike Samson Stefan Devan Emmitt Fletcher Jaidyn Remy Casen Houston Santos Seamus Jedidiah Major Vincenzo Gaige Winston Aedan " +
            "Deon Jaycob Kamryn Quinten Darnell Jaxen Deegan Landry Humberto Jadyn Salvatore Aarush Edison Kadyn Abdullah Alfred Ameer " +
            "Carsen Jaydin Lionel Howard Davon Eden Trystan Zaire Johann Antwan Bodhi Jayvion Marley Theo Bridger Donte Lennon Irvin Yael " +
            "Jencarlos Arnav Devyn Ernest Ignacio Leighton Leonidas Octavio Rayden Hezekiah Ross Hayes Lennox Nigel Vaughn Anders Keon " +
            "Dario Leroy Cortez Darryl Jakobe Koen Darien Haiden Legend Tyrese Zaid Dangelo Maxx Pierre Camdyn Chaim Damari Sonny Antony " +
            "Blaise Cain Pranav Roderick Yadiel Eliot Hugh Broderick Lathan Makhi Ronaldo Ralph Zack Kael Keyon Kingsley Talan Yair " +
            "Demarion Gibson Reagan Cristofer Daylen Jordon Dashawn Masen Clarence Dillan Kadin Rowen Thaddeus Yousef Clinton Sheldon " +
            "Slade Joziah Keshawn Menachem Bailey Camilo Destin Jaquan Jaydan Crew";

        #endregion

        #region LastNames

        string TEMP_male_lastNames_data =
            "Smith Johnson Williams Jones Brown Davis Miller Wilson Moore Taylor Anderson Thomas Jackson " +
            "White Harris Martin Thompson Garcia Martinez Robinson Clark Rodriguez Lewis Lee Walker Hall Allen Young Hernandez King " +
            "Wright Lopez Hill Scott Green Adams Baker Gonzalez Nelson Carter Mitchell Perez Roberts Turner Phillips Campbell Parker " +
            "Evans Edwards Collins Stewart Sanchez Morris Rogers Reed Cook Morgan Bell Murphy Bailey Rivera Cooper Richardson Cox Howard " +
            "Ward Torres Peterson Gray Ramirez James Watson Brooks Kelly Sanders Price Bennett Wood Barnes Ross Henderson Coleman " +
            "Jenkins Perry Powell Long Patterson Hughes Flores Washington Butler Simmons Foster Gonzales Bryant Alexander Russell " +
            "Griffin Diaz Hayes Myers Ford Hamilton Graham Sullivan Wallace Woods Cole West Jordan Owens Reynolds Fisher Ellis " +
            "Harrison Gibson Mcdonald Cruz Marshall Ortiz Gomez Murray Freeman Wells Webb Simpson Stevens Tucker Porter Hunter Hicks " +
            "Crawford Henry Boyd Mason Morales Kennedy Warren Dixon Ramos Reyes Burns Gordon Shaw Holmes Rice Robertson Hunt Black " +
            "Daniels Palmer Mills Nichols Grant Knight Ferguson Rose Stone Hawkins Dunn Perkins Hudson Spencer Gardner Stephens Payne " +
            "Pierce Berry Matthews Arnold Wagner Willis Ray Watkins Olson Carroll Duncan Snyder Hart Cunningham Bradley Lane Andrews " +
            "Ruiz Harper Fox Riley Armstrong Carpenter Weaver Greene Lawrence Elliott Chavez Sims Austin Peters Kelley Franklin Lawson " +
            "Fields Gutierrez Ryan Schmidt Carr Vasquez Castillo Wheeler Chapman Oliver Montgomery Richards Williamson Johnston " +
            "Banks Meyer Bishop Mccoy Howell Alvarez Morrison Hansen Fernandez Garza Harvey Little Burton Stanley Nguyen George " +
            "Jacobs Reid Kim Fuller Lynch Dean Gilbert Garrett Romero Welch Larson Frazier Burke Hanson Day Mendoza Moreno Bowman " +
            "Medina Fowler Brewer Hoffman Carlson Silva Pearson Holland Douglas Fleming Jensen Vargas Byrd Davidson Hopkins May " +
            "Terry Herrera Wade Soto Walters Curtis Neal Caldwell Lowe Jennings Barnett Graves Jimenez Horton Shelton Barrett Castro " +
            "Sutton Gregory Mckinney Lucas Miles Craig Rodriquez Chambers Holt Lambert Fletcher Watts Bates Hale Rhodes Pena Beck " +
            "Newman Haynes Mcdaniel Mendez Bush Vaughn Parks Dawson Santiago Norris Hardy Love Steele Curry Powers Schultz Barker " +
            "Guzman Page Munoz Ball Keller Chandler Weber Leonard Walsh Lyons Ramsey Wolfe Schneider Mullins Benson Sharp Bowen Daniel " +
            "Barber Cummings Hines Baldwin Griffith Valdez Hubbard Salazar Reeves Warner Stevenson Burgess Santos Tate Cross Garner " +
            "Mann Mack Moss Thornton Dennis Mcgee Farmer Delgado Aguilar Vega Glover Manning Cohen Harmon Rodgers Robbins Newton Todd " +
            "Blair Higgins Ingram Reese Cannon Strickland Townsend Potter Goodwin Walton Rowe Hampton Ortega Patton Swanson Joseph " +
            "Francis Goodman Maldonado Yates Becker Erickson Hodges Rios Conner Adkins Webster Norman Malone Hammond Flowers Cobb " +
            "Moody Quinn Blake Maxwell Pope Floyd Osborne Paul Mccarthy Guerrero Lindsey Estrada Sandoval Gibbs Tyler Gross Fitzgerald " +
            "Stokes Doyle Sherman Saunders Wise Colon Gill Alvarado Greer Padilla Simon Waters Nunez Ballard Schwartz Mcbride " +
            "Houston Christensen Klein Pratt Briggs Parsons Mclaughlin Zimmerman French Buchanan Moran Copeland Roy Pittman Brady " +
            "Mccormick Holloway Brock Poole Frank Logan Owen Bass Marsh Drake Wong Jefferson Park Morton Abbott Sparks Patrick " +
            "Norton Huff Clayton Massey Lloyd Figueroa Carson Bowers Roberson Barton Tran Lamb Harrington Casey Boone Cortez Clarke " +
            "Mathis Singleton Wilkins Cain Bryan Underwood Hogan Mckenzie Collier Luna Phelps Mcguire Allison Bridges Wilkerson " +
            "Nash Summers Atkins Wilcox Pitts Conley Marquez Burnett Richard Cochran Chase Davenport Hood Gates Clay Ayala Sawyer " +
            "Roman Vazquez Dickerson Hodge Acosta Flynn Espinoza Nicholson Monroe Wolf Morrow Kirk Randall Anthony Whitaker Skinner " +
            "Ware Molina Kirby Huffman Bradford Charles Gilmore Dominguez Bruce Lang Combs Kramer Heath Hancock Gallagher Gaines " +
            "Shaffer Short Wiggins Mathews Mcclain Fischer Wall Small Melton Hensley Bond Dyer Cameron Grimes Contreras Christian " +
            "Wyatt Baxter Snow Mosley Shepherd Larsen Hoover Beasley Glenn Petersen Whitehead Meyers Keith Garrison Vincent Shields " +
            "Horn Savage Olsen Schroeder Hartman Woodard Mueller Kemp Deleon Booth Patel Calhoun Wiley Eaton Cline Navarro Harrell " +
            "Lester Humphrey Parrish Duran Hutchinson Hess Dorsey Bullock Robles Beard Dalton Avila Vance Rich Blackwell York Johns " +
            "Blankenship Trevino Salinas Campos Pruitt Moses Callahan Golden Montoya Hardin Guerra Mcdowell Carey Stafford Gallegos " +
            "Henson Wilkinson Booker Merritt Miranda Atkinson Orr Decker Hobbs Preston Tanner Knox Pacheco Stephenson Glass Rojas " +
            "Serrano Marks Hickman English Sweeney Strong Prince Mcclure Conway Walter Roth Maynard Farrell Lowery Hurst Nixon Weiss " +
            "Trujillo Ellison Sloan Juarez Winters Mclean Randolph Leon Boyer Villarreal Mccall Gentry Carrillo Kent Ayers Lara Shannon " +
            "Sexton Pace Hull Leblanc Browning Velasquez Leach Chang House Sellers Herring Noble Foley Bartlett Mercado Landry Durham " +
            "Walls Barr Mckee Bauer Rivers Everett Bradshaw Pugh Velez Rush Estes Dodson Morse Sheppard Weeks Camacho Bean Barron " +
            "Livingston Middleton Spears Branch Blevins Chen Kerr Mcconnell Hatfield Harding Ashley Solis Herman Frost Giles Blackburn " +
            "William Pennington Woodward Finley Mcintosh Koch Best Solomon Mccullough Dudley Nolan Blanchard Rivas Brennan Mejia " +
            "Kane Benton Joyce Buckley Haley Valentine Maddox Russo Mcknight Buck Moon Mcmillan Crosby Berg Dotson Mays Roach Church " +
            "Chan Richmond Meadows Faulkner Knapp Kline Barry Ochoa Jacobson Gay Avery Hendricks Horne Shepard Hebert Cherry Cardenas " +
            "Mcintyre Whitney Waller Holman Donaldson Cantu Terrell Morin Gillespie Fuentes Tillman Sanford Bentley Peck Key Salas " +
            "Rollins Gamble Dickson Battle Santana Cabrera Cervantes Howe Hinton Hurley Spence Zamora Yang Mcneil Suarez Case Petty " +
            "Gould Mcfarland Sampson Carver Bray Rosario Macdonald Stout Hester Melendez Dillon Farley Hopper Galloway Potts Bernard " +
            "Joyner Stein Aguirre Osborn Mercer Bender Franco Rowland Sykes Benjamin Travis Pickett Crane Sears Mayo Dunlap Hayden " +
            "Wilder Mckay Coffey Mccarty Ewing Cooley Vaughan Bonner Cotton Holder Stark Ferrell Cantrell Fulton Lynn Lott Calderon " +
            "Rosa Pollard Hooper Burch Mullen Fry Riddle Levy David Duke Guy Michael Britt Frederick Daugherty Berger Dillard Alston " +
            "Jarvis Frye Riggs Chaney Odom Duffy Fitzpatrick Valenzuela Merrill Mayer Alford Mcpherson Acevedo Donovan Barrera Albert " +
            "Cote Reilly Compton Raymond Mooney Mcgowan Craft Cleveland Clemons Wynn Nielsen Baird Stanton Snider Rosales Bright Witt " +
            "Stuart Hays Holden Rutledge Kinney Clements Castaneda Slater Hahn Emerson Conrad Burks Delaney Pate Lancaster Sweet " +
            "Justice Tyson Sharpe Whitfield Talley Macias Irwin Burris Ratliff Mccray Madden Kaufman Beach Goff Cash Bolton Mcfadden " +
            "Levine Good Byers Kirkland Kidd Workman Carney Dale Mcleod Holcomb England Finch Head Burt Hendrix Sosa Haney Franks " +
            "Sargent Nieves Downs Rasmussen Bird Hewitt Lindsay Delacruz Vinson Dejesus Hyde Forbes Gilliam Guthrie Wooten Huber " +
            "Barlow Boyle Mcmahon Buckner Rocha Puckett Langley Knowles Cooke Velazquez Whitley Noel Vang";

        #endregion

        string[] TEMP_male_firstNames = TEMP_male_firstNames_data.Split(' ');
        string[] TEMP_male_lastNames = TEMP_male_lastNames_data.Split(' ');
        return TEMP_male_firstNames[UnityEngine.Random.Range(0, TEMP_male_firstNames.Length - 1)] + "" +
               TEMP_male_lastNames[UnityEngine.Random.Range(0, TEMP_male_lastNames.Length - 1)];
    }

    public static float GetTimeDelayReloadAds(ref int retryAttempt)
    {
        retryAttempt++;
        return (float)Math.Pow(2, Math.Min(5, retryAttempt));
    }


    #endregion

    #region[ Call ADS ]
    public static void ShowBanner()
    {
        if (VariableSystem.RemoveAdsHack || VariableSystem.RemoveAds) return;

        CC_Interface.instance.ShowBanner();
    }
    public static void DestroyBanner()
    {
        CC_Interface.instance.DestroyBanner();
    }
    public static void ShowBannerCollapse(bool isShowLoading, UnityAction callback = null)
    {
        callback?.Invoke();
        AdsConfig.instance.ShowBannerCollapseNow(isShowLoading, callback);
    }

    public static void ShowCMP(UnityAction callback = null)
    {
        if (CC_Ads.instance == null)
            Debug.Log("cc ads null");

        CC_Ads.instance.LevelShowCMP(callback);
    }
    public static void CheckShowInter(string idAds, Action<bool> callback = null, bool isCountTime = true)
    {
        if (VariableSystem.RemoveAdsHack || VariableSystem.RemoveAds || !VariableSystem.IsCanShowInter)
        {
            callback?.Invoke(true);
            return;
        }

        if (!IsEditor())
        {
            if (isCountTime)
            {
                if (AdsConfig.instance != null)
                    AdsConfig.instance.CheckShowInter(idAds, callback);
                else
                    CC_Interface.instance.ShowInter(idAds, callback);
            }
            else
                CC_Interface.instance.ShowInter(idAds, callback);
        }
        else
            callback?.Invoke(true);
    }
    public static void ShowRewardAds(string idAds, string where = "", Action<bool> callback = null)
    {

        LogEvent(KeyLogFirebase.AdsRewardClick + where);
        if (!IsEditor())
        {
            if (VariableSystem.RemoveAdsHack)
            {
                InitDataGame.instance.listDataDailyQuest[5].CountFinishQuest++;
                callback?.Invoke(true);
            }
            else
                CC_Interface.instance.ShowReward(idAds, where, callback);
        }
        else
        {
            InitDataGame.instance.listDataDailyQuest[5].CountFinishQuest++;
            callback?.Invoke(true);
        }
    }
    public static void BuyIAP(IAP_Product iAP_Product = IAP_Product.RemoveAds, Action<bool> callback = null)
    {

        if (!IsEditor())
        {
            if (VariableSystem.RemoveAdsHack)
            {
                CC_Interface.instance.RestorePurchase(iAP_Product);
                callback?.Invoke(true);
            }
            else
            {
                CC_Interface.instance.BuyIAP(iAP_Product, callback);
                //callback?.Invoke(true);
            }
        }
        else
        {
            CC_Interface.instance.RestorePurchase(iAP_Product);
            callback?.Invoke(true);
        }
    }
    public static void TrackRevenue_Event(object obj, TypeAds typeAds, AdValue adValue)
    {
        if (IsEditor()) return;

        long valueMicros = adValue.Value;
        string currencyCode = adValue.CurrencyCode;
        PrecisionType precision = adValue.Precision;
        double revenueAmount = valueMicros / 1000000f;
        ResponseInfo responseInfo = null;
        if (obj is RewardedAd rewardedAd)
        {
            responseInfo = rewardedAd.GetResponseInfo();
        }
        if (obj is InterstitialAd inter)
        {
            responseInfo = inter.GetResponseInfo();
        }
        if (obj is BannerView banner)
        {
            responseInfo = banner.GetResponseInfo();
        }
        if (obj is AppOpenAd aoa)
        {
            responseInfo = aoa.GetResponseInfo();
        }
        if (obj is NativeAd nativeAds)
        {
            responseInfo = nativeAds.GetResponseInfo();
        }
        if (obj is BannerView bannerCollap)
        {
            responseInfo = bannerCollap.GetResponseInfo();
        }
        string responseId = responseInfo.GetResponseId();

        AdapterResponseInfo loadedAdapterResponseInfo = responseInfo.GetLoadedAdapterResponseInfo();
        string adSourceId = loadedAdapterResponseInfo.AdSourceId;
        string adSourceInstanceId = loadedAdapterResponseInfo.AdSourceInstanceId;
        string adSourceInstanceName = loadedAdapterResponseInfo.AdSourceInstanceName;
        string adSourceName = loadedAdapterResponseInfo.AdSourceName;
        string adapterClassName = loadedAdapterResponseInfo.AdapterClassName;
        long latencyMillis = loadedAdapterResponseInfo.LatencyMillis;
        Dictionary<string, string> credentials = loadedAdapterResponseInfo.AdUnitMapping;

        Dictionary<string, string> extras = responseInfo.GetResponseExtras();
        string mediationGroupName = extras["mediation_group_name"];
        string mediationABTestName = extras["mediation_ab_test_name"];
        string mediationABTestVariant = extras["mediation_ab_test_variant"];

        string eventName = "AdRevenue"; // Tên sự kiện
        string productSKU = adSourceId; // Mã sản phẩm là ID nguồn quảng cáo
        string productName = adSourceName; // Tên sản phẩm là tên nguồn quảng cáo
        string productCategory = "Ad"; // Danh mục sản phẩm
        int productQuantity = 1; // Số lượng mặc định là 1
        double productPrice = revenueAmount; // Giá mỗi sản phẩm (bằng tổng doanh thu)

        Debug.Log("Start Loc Revenue ");
        SingularSDK.CustomRevenue(eventName, currencyCode, revenueAmount, productSKU, productName, productCategory, productQuantity, productPrice);
        Debug.Log($"Ad Revenue Event Sent: {eventName}, Currency: {currencyCode}, Revenue: {revenueAmount}, Source: {adSourceName}");


        Debug.Log("TrackRevenue_Event _ Type ads = " + typeAds + "|| Value = " + ((double)adValue.Value / 1000000));
        Firebase.Analytics.Parameter[] AdParameters = {
             new Firebase.Analytics.Parameter("ad_platform", "GoogleAdmob"),
              new Firebase.Analytics.Parameter("currency","USD"),
            new Firebase.Analytics.Parameter("value", revenueAmount)
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent(typeAds.ToString(), AdParameters);
    }
    public static void TrackRevenueIAP(string currency, double price)
    {
        SingularSDK.CustomRevenue(TypeAds.IAP.ToString(), currency, price);
        Firebase.Analytics.Parameter[] AdParameters = {
             new Firebase.Analytics.Parameter("ad_platform", "GoogleAdmob"),
              new Firebase.Analytics.Parameter("currency",currency),
            new Firebase.Analytics.Parameter("value",price)
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent(TypeAds.IAP.ToString(), AdParameters);
    }

    #endregion

    #region [ Check Plasform ]
    /// <summary>
    /// Check On Editor
    /// </summary>
    /// <returns>True if "On Editor" Else "False"</returns>
    public static bool IsEditor()
    {
#if UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }
    /// <summary>
    ///  Check On IOS
    /// </summary>
    /// <returns>True if "On IOS" Else "False"</returns>
    public static bool IsIOS()
    {
#if UNITY_IOS
        return true;
#else
        return false;
#endif
    }
    /// <summary>
    ///  Check On Android
    /// </summary>
    /// <returns>True if "On Android" Else "False"</returns>
    public static bool IsAndroid()
    {
#if UNITY_ANDROID
        return true;
#else
        return false;
#endif
    }
    #endregion

    #region [Log Firebase]

    public static void LogEvent(string eventName)
    {
        //., $, #, [, ], /

        string str = "Ver" + Application.version + "_" + eventName;
        str = str.Replace(",", "");
        str = str.Replace(".", "");
        str = str.Replace("$", "_");
        str = str.Replace("#", "_");
        str = str.Replace("[", "_");
        str = str.Replace("]", "_");
        str = str.Replace("(", "");
        str = str.Replace(")", "");
        str = str.Replace("/", "_");
        Debug.Log("key log = " + str);
        if (!IsEditor())
            //     if (KeepObject.instance.mode == TypeMode.Release)
            CC_LogFirebase.instance.LogEventWithString(str);
    }

    #endregion
}
public class RandomList<T>
{
    public List<T> Range(List<T> listRandom, int countRandom)
    {
        List<T> list = new List<T>();
        List<T> listTmp = new List<T>();

        foreach (var item in listRandom)
            listTmp.Add(item);

        if (listTmp.Count >= countRandom)
        {
            while (list.Count < countRandom)
            {
                T t = listTmp[UnityEngine.Random.Range(0, listTmp.Count)];
                list.Add(t);
                listTmp.Remove(t);
            }
        }

        return list;
    }
    public List<T> Mix(List<T> listMix)
    {
        List<T> list = new List<T>();
        List<T> listTmp = new List<T>();

        foreach (var item in listMix)
            listTmp.Add(item);

        if (list.Count < listMix.Count)
        {
            while (list.Count < listMix.Count)
            {
                T t = listTmp[UnityEngine.Random.Range(0, listTmp.Count)];
                list.Add(t);
                listTmp.Remove(t);
            }
        }

        return list;
    }

}
