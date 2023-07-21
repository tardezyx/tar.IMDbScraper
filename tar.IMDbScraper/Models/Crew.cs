using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Crew {
    public List<Person> Additional           { get; set; } = new List<Person>();
    public List<Person> Animation            { get; set; } = new List<Person>();
    public List<Person> Art                  { get; set; } = new List<Person>();
    public List<Person> ArtDirectionBy       { get; set; } = new List<Person>();
    public List<Person> AssistantDirection   { get; set; } = new List<Person>();
    public List<Person> CameraAndElectrical  { get; set; } = new List<Person>();
    public List<Person> Cast                 { get; set; } = new List<Person>();
    public List<Person> Casting              { get; set; } = new List<Person>();
    public List<Person> CastingBy            { get; set; } = new List<Person>();
    public List<Person> CinematographyBy     { get; set; } = new List<Person>();
    public List<Person> CostumeAndWardrobe   { get; set; } = new List<Person>();
    public List<Person> CostumeDesignBy      { get; set; } = new List<Person>();
    public List<Person> DirectedBy           { get; set; } = new List<Person>();
    public List<Person> EditingBy            { get; set; } = new List<Person>();
    public List<Person> Editorial            { get; set; } = new List<Person>();
    public List<Person> LocationManagement   { get; set; } = new List<Person>();
    public List<Person> MakeUp               { get; set; } = new List<Person>();
    public List<Person> Music                { get; set; } = new List<Person>();
    public List<Person> MusicBy              { get; set; } = new List<Person>();
    public List<Person> Others               { get; set; } = new List<Person>();
    public List<Person> ProducedBy           { get; set; } = new List<Person>();
    public List<Person> ProductionDesignBy   { get; set; } = new List<Person>();
    public List<Person> ProductionManagement { get; set; } = new List<Person>();
    public List<Person> ScriptAndContinuity  { get; set; } = new List<Person>();
    public List<Person> SetDecorationBy      { get; set; } = new List<Person>();
    public List<Person> Sound                { get; set; } = new List<Person>();
    public List<Person> SpecialEffects       { get; set; } = new List<Person>();
    public List<Person> Stunts               { get; set; } = new List<Person>();
    public List<Person> Thanks               { get; set; } = new List<Person>();
    public List<Person> Transportation       { get; set; } = new List<Person>();
    public List<Person> VisualEffects        { get; set; } = new List<Person>();
    public List<Person> WrittenBy            { get; set; } = new List<Person>();
  }
}