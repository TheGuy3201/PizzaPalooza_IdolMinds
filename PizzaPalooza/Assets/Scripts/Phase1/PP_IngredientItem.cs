using UnityEngine;

public class PP_IngredientItem : MonoBehaviour
{
    [SerializeField] private PP_IngredientType ingredientType;

    public PP_IngredientType IngredientType => ingredientType;
}
