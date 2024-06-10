using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityProfile
{
    public string Name { get; set; }
    public Texture2D Image { get; set; }

    public EntityProfile(string name, Texture2D image)
    {
        Name = name;
        Image = image;
    }
}

