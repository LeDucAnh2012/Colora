using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RandomByWeight
{
    public int randomObjectIndex;
    float[] Example;
    static float[] arrayOfRandom;

    public static int Choose(float[] probs)
    {
        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }

        return probs.Length - 1;
    }

    public static int Choose(List<float> probs)
    {
        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Count; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }

        return probs.Count - 1;
    }


    //Spawn a int num by probality that contain in float[] prob 	
    public static int SpawnByWeight(float[] prob)
    {
        int value = Choose(prob);

        return value;
    }
    public static int SpawnByWeight(List<float> prob)
    {
        int value = Choose(prob);

        return value;
    }

    public static int RandomFourElement(float _value, float _value2, float _val3, float _val4)
    {
        int rd = 0;
        arrayOfRandom = new float[4];
        arrayOfRandom[0] = _value;
        arrayOfRandom[1] = _value2;
        arrayOfRandom[2] = _val3;
        arrayOfRandom[3] = _val4;
        return rd = Choose(arrayOfRandom);
    }


    public static int RandomTwoElement(float _value1, float _value2)
    {
        int rd = 0;
        arrayOfRandom = new float[2];
        arrayOfRandom[0] = _value1;
        arrayOfRandom[1] = _value2;
        return rd = Choose(arrayOfRandom);
    }

    void arrayCreate(float[] newPro, int numOfElm)
    {
        newPro = new float[numOfElm];
        //return newPro;
    }

    void ExampleRandom()
    {
        Example = new float[4];
        Example[0] = 10;
        Example[1] = 20;
        Example[2] = 30;
        Example[3] = 40;
        SpawnByWeight(Example);
        Debug.Log(randomObjectIndex);
    }
}