using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PP_OrderManager : MonoBehaviour
{
    [Serializable]
    public class PP_Order
    {
        public int orderId;
        public bool requireSauce = true;
        public List<PP_IngredientType> toppings = new List<PP_IngredientType>();
        public float patience = 1f;
        public float patienceDecay = 0.025f;
    }

    [SerializeField] private PP_GameManager gameManager;
    [SerializeField] private int maxConcurrentOrders = 3;
    [SerializeField] private float baseSpawnInterval = 18f;
    [SerializeField] private float minSpawnInterval = 8f;

    [Header("Recipe Pool")]
    [SerializeField] private List<PP_IngredientType> toppingPool = new List<PP_IngredientType>
    {
        PP_IngredientType.Cheese,
        PP_IngredientType.Pepperoni,
        PP_IngredientType.Mushroom,
        PP_IngredientType.Onions,
        PP_IngredientType.Olives
    };

    private readonly List<PP_Order> activeOrders = new List<PP_Order>();
    private int nextOrderId = 1;
    private float spawnTimer;

    public IReadOnlyList<PP_Order> ActiveOrders => activeOrders;

    private void Awake()
    {
        // Initialize toppingPool if it's empty (in case serialization didn't work)
        if (toppingPool.Count == 0)
        {
            toppingPool.AddRange(new[]
            {
                PP_IngredientType.Cheese,
                PP_IngredientType.Pepperoni,
                PP_IngredientType.Mushroom,
                PP_IngredientType.Onions,
                PP_IngredientType.Olives
            });
        }
    }

    private void Start()
    {
        spawnTimer = 0f;
        while (activeOrders.Count < 1)
        {
            SpawnOrder();
        }
    }

    private void Update()
    {
        if (gameManager == null || gameManager.IsGameOver)
        {
            return;
        }

        UpdateSpawn(Time.deltaTime);
        UpdatePatience(Time.deltaTime);
    }

    public string GetOrdersDisplayText()
    {
        if (activeOrders.Count == 0)
        {
            return "No active orders";
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < activeOrders.Count; i++)
        {
            PP_Order order = activeOrders[i];
            sb.Append("#").Append(order.orderId).Append(" | Patience: ")
                .Append(Mathf.RoundToInt(order.patience * 100f)).Append("% | ");

            bool hasDisplayedIngredient = false;
            if (order.requireSauce)
            {
                sb.Append("Sauce");
                hasDisplayedIngredient = true;
            }

            for (int j = 0; j < order.toppings.Count; j++)
            {
                if (hasDisplayedIngredient)
                {
                    sb.Append(" + ");
                }

                sb.Append(order.toppings[j]);
                hasDisplayedIngredient = true;
            }

            if (!hasDisplayedIngredient)
            {
                sb.Append("Plain");
            }

            if (i < activeOrders.Count - 1)
            {
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    public bool TryDeliver(PP_Pizza pizza, out int scoreDelta, out int tipDelta, out string feedback, out bool isCorrectDelivery)
    {
        scoreDelta = 0;
        tipDelta = 0;
        feedback = "No order matched";
        isCorrectDelivery = false;

        if (pizza == null || activeOrders.Count == 0)
        {
            return false;
        }

        PP_Order bestOrder = null;
        float bestMatch = -1f;
        for (int i = 0; i < activeOrders.Count; i++)
        {
            float match = ComputeMatch(activeOrders[i], pizza);
            if (match > bestMatch)
            {
                bestMatch = match;
                bestOrder = activeOrders[i];
            }
        }

        if (bestOrder == null)
        {
            return false;
        }

        if (pizza.State == PP_Pizza.CookState.Raw)
        {
            gameManager.NotifyOrderFailed("Pizza was undercooked");
            feedback = "Undercooked pizza";
            return false;
        }

        if (pizza.State == PP_Pizza.CookState.Burnt)
        {
            gameManager.NotifyOrderFailed("Pizza was burnt");
            feedback = "Burnt pizza";
            return false;
        }

        bool goodOrder = bestMatch >= 0.75f;

        if (!goodOrder)
        {
            gameManager.NotifyOrderFailed("Order was incorrect");
            feedback = "Wrong recipe";
            return false;
        }

        if (!pizza.TryMarkDelivered())
        {
            feedback = "Pizza already submitted";
            return false;
        }

        activeOrders.Remove(bestOrder);

        scoreDelta = Mathf.RoundToInt(100f * bestMatch);
        tipDelta = Mathf.RoundToInt(40f * bestOrder.patience * bestMatch);
        gameManager.NotifyOrderCompleted(scoreDelta, tipDelta, bestOrder.patience);
        feedback = "Order complete";
        isCorrectDelivery = true;
        return true;
    }

    private void UpdateSpawn(float deltaTime)
    {
        if (activeOrders.Count >= maxConcurrentOrders)
        {
            return;
        }

        spawnTimer += deltaTime;
        float pressure = gameManager.GetPressureMultiplier();
        float currentInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval / pressure);
        if (spawnTimer >= currentInterval)
        {
            spawnTimer = 0f;
            SpawnOrder();
        }
    }

    private void UpdatePatience(float deltaTime)
    {
        float pressure = gameManager.GetPressureMultiplier();
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            PP_Order order = activeOrders[i];
            order.patience -= order.patienceDecay * pressure * deltaTime;
            if (order.patience <= 0f)
            {
                activeOrders.RemoveAt(i);
                gameManager.NotifyOrderFailed("Customer left due to low patience");
            }
        }
    }

    private void SpawnOrder()
    {
        PP_Order order = null;
        const int maxRecipeAttempts = 64;

        for (int attempt = 0; attempt < maxRecipeAttempts; attempt++)
        {
            PP_Order candidate = CreateRandomOrder();
            if (!HasDuplicateActiveRecipe(candidate))
            {
                order = candidate;
                break;
            }
        }

        if (order == null)
        {
            order = CreateRandomOrder();
        }

        order.orderId = nextOrderId++;
        activeOrders.Add(order);
    }

    private PP_Order CreateRandomOrder()
    {
        PP_Order order = new PP_Order
        {
            patience = UnityEngine.Random.Range(0.75f, 1f),
            patienceDecay = UnityEngine.Random.Range(0.008f, 0.015f)
        };

        int toppingsCount = UnityEngine.Random.Range(1, 4);
        List<PP_IngredientType> poolCopy = new List<PP_IngredientType>(toppingPool);
        for (int i = 0; i < toppingsCount && poolCopy.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, poolCopy.Count);
            order.toppings.Add(poolCopy[index]);
            poolCopy.RemoveAt(index);
        }

        return order;
    }

    private bool HasDuplicateActiveRecipe(PP_Order candidate)
    {
        for (int i = 0; i < activeOrders.Count; i++)
        {
            if (HaveSameRecipe(activeOrders[i], candidate))
            {
                return true;
            }
        }

        return false;
    }

    private bool HaveSameRecipe(PP_Order a, PP_Order b)
    {
        if (a == null || b == null)
        {
            return false;
        }

        if (a.requireSauce != b.requireSauce)
        {
            return false;
        }

        if (a.toppings.Count != b.toppings.Count)
        {
            return false;
        }

        for (int i = 0; i < a.toppings.Count; i++)
        {
            if (!OrderHasTopping(b, a.toppings[i]))
            {
                return false;
            }
        }

        return true;
    }

    private float ComputeMatch(PP_Order order, PP_Pizza pizza)
    {
        if (pizza == null || !pizza.HasDough)
        {
            return 0f;
        }

        float score = 0f;
        
        // Dough is required
        if (pizza.HasDough)
        {
            score += 0.3f;
        }

        // Sauce requirement check - strict
        if (order.requireSauce && !pizza.HasSauce)
        {
            return 0f; // Fail immediately if sauce is required but missing
        }
        score += 0.2f;

        // Toppings must be exact match (no extra toppings)
        int matchedToppings = 0;
        for (int i = 0; i < order.toppings.Count; i++)
        {
            if (PizzaHasTopping(pizza, order.toppings[i]))
            {
                matchedToppings++;
            }
        }

        // Check for extra toppings (fail if pizza has toppings not in order)
        int extraToppings = 0;
        foreach (PP_IngredientType existing in pizza.Toppings)
        {
            if (!OrderHasTopping(order, existing))
            {
                extraToppings++;
            }
        }

        if (extraToppings > 0)
        {
            return 0f; // Fail if pizza has extra toppings
        }

        float toppingScore = order.toppings.Count > 0 ? (float)matchedToppings / order.toppings.Count : 1f;
        score += 0.5f * toppingScore;
        return Mathf.Clamp01(score);
    }

    private bool OrderHasTopping(PP_Order order, PP_IngredientType topping)
    {
        foreach (PP_IngredientType required in order.toppings)
        {
            if (required == topping)
            {
                return true;
            }
        }
        return false;
    }

    private bool PizzaHasTopping(PP_Pizza pizza, PP_IngredientType topping)
    {
        foreach (PP_IngredientType existing in pizza.Toppings)
        {
            if (existing == topping)
            {
                return true;
            }
        }

        return false;
    }
}
