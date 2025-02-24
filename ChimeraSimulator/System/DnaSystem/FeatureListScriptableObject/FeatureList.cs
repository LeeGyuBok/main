using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatureList", menuName = "Scriptable Objects/FeatureList")]
public class FeatureList : ScriptableObject
{
    [SerializeField] private GeneType geneType;
    public GeneType GeneType => geneType;
    [SerializeField] private Feature[] features = new Feature[3];
    public Feature[] Features => features;
    [SerializeField] private string description;
    public string Description => description;
}
