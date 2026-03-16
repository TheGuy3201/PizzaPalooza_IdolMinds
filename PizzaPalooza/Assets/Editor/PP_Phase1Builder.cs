using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PP_Phase1Builder
{
    private const string PrefabFolder = "Assets/Prefabs/Phase1";
    private const string PizzaPrefabPath = PrefabFolder + "/Pizza_Base.prefab";
    private const string CustomPizzaPrefabPath = "Assets/Prefabs/PizzaVisual.prefab";

    [MenuItem("PizzaPalooza/Build Phase 1 (Step 9+) Setup")]
    public static void BuildStep9Onward()
    {
        EnsureFolder(PrefabFolder);

        PP_HUDBuilder.BuildGameplayHud();

        GameObject systems = FindOrCreate("GameSystems");
        PP_GameManager gm = GetOrAdd<PP_GameManager>(systems);
        PP_OrderManager om = GetOrAdd<PP_OrderManager>(systems);

        GameObject hudObject = FindOrCreate("HUDController");
        PP_HUDController hud = GetOrAdd<PP_HUDController>(hudObject);

        SetRef(gm, "hud", hud);
        SetRef(om, "gameManager", gm);

        TrySetupPlayerInteractor();

        PP_Pizza pizzaPrefab = EnsurePizzaPrefab();
        Dictionary<PP_IngredientType, PP_PickupItem> ingredientPrefabs = EnsureIngredientPrefabs();

        var (assembly, assemblyIsNew) = FindOrCreateTracked("AssemblyStation");
        var (oven, ovenIsNew) = FindOrCreateTracked("OvenStation");
        var (delivery, deliveryIsNew) = FindOrCreateTracked("DeliveryCounter");

        if (assemblyIsNew) assembly.transform.position = new Vector3(0f, 0.9f, 2.5f);
        if (ovenIsNew) oven.transform.position = new Vector3(2.5f, 0.9f, 2.5f);
        if (deliveryIsNew) delivery.transform.position = new Vector3(-2.5f, 0.9f, 2.5f);

        EnsureBoxCollider(assembly, new Vector3(1.8f, 1.8f, 1.2f), new Vector3(0f, 0.6f, 0f));
        EnsureBoxCollider(oven, new Vector3(1.5f, 1f, 1f));
        EnsureBoxCollider(delivery, new Vector3(1.5f, 1f, 1f));

        PP_AssemblyStation assemblyStation = GetOrAdd<PP_AssemblyStation>(assembly);
        PP_OvenStation ovenStation = GetOrAdd<PP_OvenStation>(oven);
        PP_DeliveryCounter deliveryCounter = GetOrAdd<PP_DeliveryCounter>(delivery);

        Transform assemblySlot = EnsureChild(assembly.transform, "StationSlot", new Vector3(0f, 0.6f, 0f));
        Transform ovenSlot = EnsureChild(oven.transform, "OvenSlot", new Vector3(0f, 0.6f, 0f));

        SetRef(assemblyStation, "pizzaPrefab", pizzaPrefab);
        SetRef(assemblyStation, "stationSlot", assemblySlot);
        SetRef(ovenStation, "ovenSlot", ovenSlot);
        SetRef(deliveryCounter, "orderManager", om);
        SetRef(deliveryCounter, "hud", hud);

        var (dispenserDough, doughIsNew) = FindOrCreateTracked("Dispenser_Dough");
        var (dispenserSauce, sauceIsNew) = FindOrCreateTracked("Dispenser_Sauce");
        var (dispenserCheese, cheeseIsNew) = FindOrCreateTracked("Dispenser_Cheese");
        var (dispenserPepperoni, pepIsNew) = FindOrCreateTracked("Dispenser_Pepperoni");
        var (dispenserMushroom, mushIsNew) = FindOrCreateTracked("Dispenser_Mushroom");

        if (doughIsNew) CreateOrUpdateDispenser(dispenserDough, new Vector3(-4.5f, 0.9f, 1f), ingredientPrefabs[PP_IngredientType.Dough]);
        else UpdateDispenser(dispenserDough, ingredientPrefabs[PP_IngredientType.Dough]);

        if (sauceIsNew) CreateOrUpdateDispenser(dispenserSauce, new Vector3(-4.5f, 0.9f, 0f), ingredientPrefabs[PP_IngredientType.Sauce]);
        else UpdateDispenser(dispenserSauce, ingredientPrefabs[PP_IngredientType.Sauce]);

        if (cheeseIsNew) CreateOrUpdateDispenser(dispenserCheese, new Vector3(-4.5f, 0.9f, -1f), ingredientPrefabs[PP_IngredientType.Cheese]);
        else UpdateDispenser(dispenserCheese, ingredientPrefabs[PP_IngredientType.Cheese]);

        if (pepIsNew) CreateOrUpdateDispenser(dispenserPepperoni, new Vector3(-4.5f, 0.9f, -2f), ingredientPrefabs[PP_IngredientType.Pepperoni]);
        else UpdateDispenser(dispenserPepperoni, ingredientPrefabs[PP_IngredientType.Pepperoni]);

        if (mushIsNew) CreateOrUpdateDispenser(dispenserMushroom, new Vector3(-4.5f, 0.9f, -3f), ingredientPrefabs[PP_IngredientType.Mushroom]);
        else UpdateDispenser(dispenserMushroom, ingredientPrefabs[PP_IngredientType.Mushroom]);

        EditorUtility.SetDirty(assemblyStation);
        EditorUtility.SetDirty(ovenStation);
        EditorUtility.SetDirty(deliveryCounter);
        EditorUtility.SetDirty(gm);
        EditorUtility.SetDirty(om);
        EditorUtility.SetDirty(hud);
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        AssetDatabase.SaveAssets();
        Debug.Log("Phase 1 step 9+ setup created. Review scene placement and assign real sprites if needed.");
    }

    private static PP_Pizza EnsurePizzaPrefab()
    {
        PP_Pizza customPizza = AssetDatabase.LoadAssetAtPath<PP_Pizza>(CustomPizzaPrefabPath);
        if (customPizza != null)
        {
            EnsurePizzaGameplayComponentsOnPrefab(CustomPizzaPrefabPath);
            return AssetDatabase.LoadAssetAtPath<PP_Pizza>(CustomPizzaPrefabPath);
        }

        GameObject customPizzaRoot = AssetDatabase.LoadAssetAtPath<GameObject>(CustomPizzaPrefabPath);
        if (customPizzaRoot != null)
        {
            EnsurePizzaGameplayComponentsOnPrefab(CustomPizzaPrefabPath);
            PP_Pizza loaded = AssetDatabase.LoadAssetAtPath<PP_Pizza>(CustomPizzaPrefabPath);
            if (loaded != null)
            {
                return loaded;
            }
        }

        Sprite sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");

        GameObject root = new GameObject("Pizza_Base");
        root.transform.position = Vector3.zero;
        EnsureBoxCollider(root, new Vector3(0.8f, 0.1f, 0.8f));
        Rigidbody rb = GetOrAdd<Rigidbody>(root);
        rb.useGravity = true;
        rb.isKinematic = false;

        GetOrAdd<PP_PickupItem>(root);
        PP_Pizza pizza = GetOrAdd<PP_Pizza>(root);
        PP_PizzaSpriteVisual visual = GetOrAdd<PP_PizzaSpriteVisual>(root);

        SpriteRenderer dough = CreateLayer(root.transform, "Base_Dough", sprite, 0, new Color(0.93f, 0.84f, 0.64f, 1f), true);
        SpriteRenderer sauce = CreateLayer(root.transform, "Layer_Sauce", sprite, 1, new Color(0.88f, 0.26f, 0.18f, 0.95f), false);
        SpriteRenderer cheese = CreateLayer(root.transform, "Layer_Cheese", sprite, 2, new Color(0.95f, 0.9f, 0.45f, 0.95f), false);
        SpriteRenderer pep = CreateLayer(root.transform, "Layer_Pepperoni", sprite, 3, new Color(0.75f, 0.2f, 0.2f, 0.9f), false);
        SpriteRenderer mush = CreateLayer(root.transform, "Layer_Mushroom", sprite, 4, new Color(0.79f, 0.68f, 0.55f, 0.85f), false);

        SetRef(visual, "doughLayer", dough);
        SetRef(visual, "sauceLayer", sauce);
        SetRef(visual, "cheeseLayer", cheese);
        SetRef(visual, "pepperoniLayer", pep);
        SetRef(visual, "mushroomLayer", mush);
        SetRef(pizza, "spriteVisual", visual);

        GameObject prefabGo = PrefabUtility.SaveAsPrefabAsset(root, PizzaPrefabPath);
        Object.DestroyImmediate(root);
        return prefabGo.GetComponent<PP_Pizza>();
    }

    private static void EnsurePizzaGameplayComponentsOnPrefab(string prefabPath)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        bool changed = false;

        PP_PickupItem pickup = root.GetComponent<PP_PickupItem>();
        if (pickup == null)
        {
            pickup = root.AddComponent<PP_PickupItem>();
            changed = true;
        }

        PP_Pizza pizza = root.GetComponent<PP_Pizza>();
        if (pizza == null)
        {
            pizza = root.AddComponent<PP_Pizza>();
            changed = true;
        }

        PP_PizzaSpriteVisual visual = root.GetComponent<PP_PizzaSpriteVisual>();
        if (visual == null)
        {
            visual = root.AddComponent<PP_PizzaSpriteVisual>();
            changed = true;
        }

        visual.AutoBindLayers();
        EditorUtility.SetDirty(visual);
        EditorUtility.SetDirty(pizza);
        EditorUtility.SetDirty(root);

        if (changed)
        {
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }

        PrefabUtility.UnloadPrefabContents(root);
        AssetDatabase.ImportAsset(prefabPath);
    }

    private static Dictionary<PP_IngredientType, PP_PickupItem> EnsureIngredientPrefabs()
    {
        Dictionary<PP_IngredientType, PP_PickupItem> map = new Dictionary<PP_IngredientType, PP_PickupItem>();
        map[PP_IngredientType.Dough] = EnsureIngredientPrefab(PP_IngredientType.Dough, new Color(0.93f, 0.84f, 0.64f, 1f));
        map[PP_IngredientType.Sauce] = EnsureIngredientPrefab(PP_IngredientType.Sauce, new Color(0.88f, 0.26f, 0.18f, 1f));
        map[PP_IngredientType.Cheese] = EnsureIngredientPrefab(PP_IngredientType.Cheese, new Color(0.95f, 0.9f, 0.45f, 1f));
        map[PP_IngredientType.Pepperoni] = EnsureIngredientPrefab(PP_IngredientType.Pepperoni, new Color(0.75f, 0.2f, 0.2f, 1f));
        map[PP_IngredientType.Mushroom] = EnsureIngredientPrefab(PP_IngredientType.Mushroom, new Color(0.79f, 0.68f, 0.55f, 1f));
        return map;
    }

    private static PP_PickupItem EnsureIngredientPrefab(PP_IngredientType type, Color color)
    {
        string path = PrefabFolder + "/Ingredient_" + type + ".prefab";
        Sprite sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        GameObject go = new GameObject("Ingredient_" + type);
        go.transform.localScale = new Vector3(0.35f, 0.08f, 0.35f);
        SpriteRenderer sr = GetOrAdd<SpriteRenderer>(go);
        sr.sprite = sprite;
        sr.color = color;
        sr.sortingOrder = 10;

        EnsureBoxCollider(go, new Vector3(1f, 1f, 1f));
        Rigidbody rb = GetOrAdd<Rigidbody>(go);
        rb.useGravity = true;
        rb.isKinematic = false;

        PP_PickupItem pickup = GetOrAdd<PP_PickupItem>(go);
        PP_IngredientItem ingredient = GetOrAdd<PP_IngredientItem>(go);
        SetEnum(ingredient, "ingredientType", (int)type);

        GameObject prefabGo = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefabGo.GetComponent<PP_PickupItem>();
    }

    private static void TrySetupPlayerInteractor()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Camera[] cams = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
            if (cams.Length > 0)
            {
                cam = cams[0];
            }
        }

        if (cam == null)
        {
            Debug.LogWarning("No camera found for PP_PlayerInteractor setup.");
            return;
        }

        GameObject playerObject = cam.transform.root.gameObject;
        PP_PlayerInteractor interactor = GetOrAdd<PP_PlayerInteractor>(playerObject);
        Transform holdPoint = cam.transform.Find("HoldPoint");
        if (holdPoint == null)
        {
            GameObject hp = new GameObject("HoldPoint");
            hp.transform.SetParent(cam.transform, false);
            hp.transform.localPosition = new Vector3(0f, -0.2f, 0.7f);
            holdPoint = hp.transform;
        }

        SetRef(interactor, "playerCamera", cam);
        SetRef(interactor, "holdPoint", holdPoint);
    }

    private static void CreateOrUpdateDispenser(GameObject dispenser, Vector3 position, PP_PickupItem ingredientPrefab)
    {
        dispenser.transform.position = position;
        EnsureBoxCollider(dispenser, new Vector3(1f, 1f, 1f));

        PP_IngredientDispenser d = GetOrAdd<PP_IngredientDispenser>(dispenser);
        Transform spawnPoint = EnsureChild(dispenser.transform, "SpawnPoint", new Vector3(0.7f, 0.4f, 0f));
        SetRef(d, "ingredientPickupPrefab", ingredientPrefab);
        SetRef(d, "spawnPoint", spawnPoint);
    }

    private static void UpdateDispenser(GameObject dispenser, PP_PickupItem ingredientPrefab)
    {
        EnsureBoxCollider(dispenser, new Vector3(1f, 1f, 1f));

        PP_IngredientDispenser d = GetOrAdd<PP_IngredientDispenser>(dispenser);
        Transform spawnPoint = EnsureChild(dispenser.transform, "SpawnPoint", new Vector3(0.7f, 0.4f, 0f));
        SetRef(d, "ingredientPickupPrefab", ingredientPrefab);
        SetRef(d, "spawnPoint", spawnPoint);
    }

    private static SpriteRenderer CreateLayer(Transform parent, string name, Sprite sprite, int order, Color color, bool enabled)
    {
        Transform existing = parent.Find(name);
        GameObject go = existing != null ? existing.gameObject : new GameObject(name);
        if (existing == null)
        {
            go.transform.SetParent(parent, false);
        }

        go.transform.localPosition = new Vector3(0f, 0f, -0.001f * order);
    go.transform.localRotation = Quaternion.identity;
    go.transform.localScale = new Vector3(0.9f, 0.9f, 1f);

        SpriteRenderer sr = GetOrAdd<SpriteRenderer>(go);
        sr.sprite = sprite;
        sr.sortingLayerName = "Default";
        sr.sortingOrder = order;
        sr.color = color;
        sr.enabled = enabled;
        return sr;
    }

    private static Transform EnsureChild(Transform parent, string name, Vector3 localPosition)
    {
        Transform child = parent.Find(name);
        if (child == null)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            child = go.transform;
        }

        child.localPosition = localPosition;
        return child;
    }

    private static void EnsureBoxCollider(GameObject go, Vector3 size)
    {
        BoxCollider bc = GetOrAdd<BoxCollider>(go);
        bc.size = size;
        bc.center = Vector3.zero;
        bc.isTrigger = false;
    }

    private static void EnsureBoxCollider(GameObject go, Vector3 size, Vector3 center)
    {
        BoxCollider bc = GetOrAdd<BoxCollider>(go);
        bc.size = size;
        bc.center = center;
        bc.isTrigger = false;
    }

    private static T GetOrAdd<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }

        return component;
    }

    private static GameObject FindOrCreate(string name)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
        {
            return existing;
        }

        return new GameObject(name);
    }

    private static (GameObject, bool) FindOrCreateTracked(string name)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
        {
            return (existing, false);
        }

        return (new GameObject(name), true);
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            Directory.CreateDirectory(fullPath);
            AssetDatabase.Refresh();
        }
    }

    private static void SetRef(Object target, string field, Object value)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(field);
        if (prop != null)
        {
            prop.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private static Object GetRef(Object target, string field)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(field);
        return prop != null ? prop.objectReferenceValue : null;
    }

    private static void SetEnum(Object target, string field, int enumValue)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(field);
        if (prop != null)
        {
            prop.enumValueIndex = enumValue;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
