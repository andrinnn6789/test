using JetBrains.Annotations;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

#pragma warning disable IDE1006 // Naming Styles
namespace IAG.VinX.Schüwo.SV.Dto;

[UsedImplicitly]
public class UploadArtAdditional
{
    public string _anbr { get; set; }
    public int _status { get; set; }
    public int? sv_standardprice { get; set; }
    public string sv_text_long_De { get; set; }
    public string manufacturer { get; set; }
    public string manufacturing_local_region { get; set; }
    public string manufacturing_country { get; set; }
    public string supplier { get; set; }
    public string supply_country { get; set; }
    public string buying_currency { get; set; }
    public bool diet_vegetarian { get; set; }
    public string ingredient_list_De { get; set; }
    public int nutritional_value_unit { get; set; }
    public int nutritional_value_amount { get; set; }
    public double nutritional_value_calories { get; set; }
    public double nutritional_value_joules { get; set; }
    public double nutritional_value_fat { get; set; }
    public double nutritional_value_saturated_fatty_acids { get; set; }
    public double nutritional_value_carbohydrates { get; set; }
    public double nutritional_value_sugar { get; set; }
    public double nutritional_value_protein { get; set; }
    public double nutritional_value_salt { get; set; }
    public bool contains_gluten { get; set; }
    public bool contains_egg { get; set; }
    public bool contains_fisch { get; set; }
    public bool contains_peanuts { get; set; }
    public bool contains_lactose { get; set; }
    public bool contains_nuts { get; set; }
    public bool contains_sellerie { get; set; }
    public bool contains_moustard { get; set; }
    public bool contains_sesame { get; set; }
    public bool contains_sulfur_dioxide_sulfites { get; set; }
    public bool contains_lupins { get; set; }
    public bool contains_alkohol { get; set; }
    public bool contains_caffeine { get; set; }
    public bool contains_taurine { get; set; }
    public bool diet_vegan { get; set; }
}
#pragma warning restore IDE1006 // Naming Styles
