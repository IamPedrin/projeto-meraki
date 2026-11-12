using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

/// <summary>
/// Script de Editor para configurar a hierarquia visual do Runner diretamente na cena (Assets/Scenes/Runner.unity).
/// Salva as alterações na própria cena, permitindo que o desenvolvedor ajuste tudo pelo Inspector do Unity.
/// </summary>
public static class ConfigurarRunnerUIScene
{
    const string ScenePath = "Assets/Scenes/Runner.unity";

    [MenuItem("Meraki/UI/Configurar Cena Runner (Salvar na Cena)")]
    public static void ConfigurarCenaRunnerNoEditor()
    {
        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.path != ScenePath)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            activeScene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        ExecutarConfiguracaoNaCena();

        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);
        Debug.Log("[ConfigurarRunnerUIScene] Cena Runner configurada e salva com sucesso no disco!");
    }

    public static void ExecutarConfiguracaoNaCena()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>(FindObjectsInactive.Include);
        if (canvas == null)
        {
            Debug.LogError("[ConfigurarRunnerUIScene] Canvas não encontrado na cena Runner!");
            return;
        }

        // Sprites necessários
        Sprite spCard = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/kenney_ui-pack-pixel-adventure/Thick outline/tile_0017.png");
        Sprite spHeader = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/kenney_ui-pack-pixel-adventure/Thick outline/tile_0016.png");
        Sprite spWood = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/kenney_ui-pack-pixel-adventure/Thick outline/tile_0001.png");
        Sprite spCream = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/kenney_ui-pack-pixel-adventure/Thick outline/tile_0000.png");
        Sprite spCoin = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Icons/Coin2.png");
        Sprite spBalanca = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Scripts/Loja/ItensDaLoja/balanca.png");

        TMP_FontAsset fontPadrao = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (fontPadrao == null)
        {
            var txtExistente = Object.FindAnyObjectByType<TextMeshProUGUI>(FindObjectsInactive.Include);
            if (txtExistente != null) fontPadrao = txtExistente.font;
        }

        Color corTextoEscuro = new Color(0.22f, 0.13f, 0.07f, 1f);
        Color corTextoClaro = new Color(1f, 0.97f, 0.92f, 1f);
        Color corMoedaDourada = new Color(0.68f, 0.42f, 0.05f, 1f); // Contraste ideal no fundo creme

        GameManager gm = Object.FindAnyObjectByType<GameManager>(FindObjectsInactive.Include);

        // ==========================================
        // 1. HUD DO JOGO (HUDGame)
        // ==========================================
        Transform hudTrans = canvas.transform.Find("HUDGame");
        if (hudTrans != null)
        {
            RectTransform hudRt = hudTrans as RectTransform;
            Undo.RecordObject(hudRt, "Ajustar HUDGame");
            hudRt.anchorMin = Vector2.zero;
            hudRt.anchorMax = Vector2.one;
            hudRt.offsetMin = Vector2.zero;
            hudRt.offsetMax = Vector2.zero;

            if (hudTrans.GetComponent<SafeArea>() == null)
                hudTrans.gameObject.AddComponent<SafeArea>();

            // Moldura Coletados (Superior Esquerdo)
            RectTransform mColetados = ObterOuCriarImg(hudTrans, "MolduraColetadosHUD",
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(24f, -20f), new Vector2(360f, 74f), spWood, true, Color.white);
            RectTransform fcCol = ObterOuCriarImg(mColetados, "FundoCreme",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(-16f, -16f), spCream, true, Color.white);
            fcCol.SetAsFirstSibling();

            // Ícone da fruta na moldura da HUD
            Sprite spFrutaIcone = null;
            var objCardTut = canvas.transform.Find("PanelTutorial/CardTutorial/ImageObjetivo");
            if (objCardTut != null && objCardTut.GetComponent<UnityEngine.UI.Image>() != null)
                spFrutaIcone = objCardTut.GetComponent<UnityEngine.UI.Image>().sprite;

            RectTransform iconeFruta = ObterOuCriarImg(mColetados, "IconeAlimentoHUD",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-130f, 0f), new Vector2(46f, 46f), spFrutaIcone, false, Color.white);
            iconeFruta.SetAsLastSibling();

            Transform txtAlimentos = hudTrans.Find("TextHUDAlimentosTotal") ?? mColetados.Find("TextHUDAlimentosTotal");
            if (txtAlimentos != null)
            {
                RectTransform taRt = txtAlimentos as RectTransform;
                Undo.RecordObject(taRt, "Ajustar TextHUDAlimentosTotal");
                taRt.SetParent(mColetados, false);
                taRt.SetAsLastSibling();
                taRt.anchorMin = new Vector2(0.5f, 0.5f);
                taRt.anchorMax = new Vector2(0.5f, 0.5f);
                taRt.pivot = new Vector2(0f, 0.5f);
                taRt.anchoredPosition = new Vector2(-96f, 0f);
                taRt.sizeDelta = new Vector2(250f, 48f);

                var tmp = txtAlimentos.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    Undo.RecordObject(tmp, "Ajustar Estilo TextHUDAlimentosTotal");
                    if (fontPadrao != null) tmp.font = fontPadrao;
                    tmp.text = "Coletados: 0";
                    tmp.fontSize = 34f;
                    tmp.enableAutoSizing = true;
                    tmp.fontSizeMin = 22f;
                    tmp.fontSizeMax = 34f;
                    tmp.fontStyle = FontStyles.Bold;
                    tmp.color = corTextoEscuro;
                    tmp.alignment = TextAlignmentOptions.MidlineLeft;
                }
            }

            // Moldura Moedas (Superior Central)
            Transform hudMoedas = hudTrans.Find("HUDMoedas");
            if (hudMoedas != null)
            {
                RectTransform hmRt = hudMoedas as RectTransform;
                Undo.RecordObject(hmRt, "Ajustar HUDMoedas");
                hmRt.anchorMin = new Vector2(0.5f, 1f);
                hmRt.anchorMax = new Vector2(0.5f, 1f);
                hmRt.pivot = new Vector2(0.5f, 1f);
                hmRt.anchoredPosition = new Vector2(0f, -20f);
                hmRt.sizeDelta = new Vector2(250f, 74f);

                var imgMoedas = hmRt.GetComponent<UnityEngine.UI.Image>();
                if (imgMoedas != null)
                {
                    Undo.RecordObject(imgMoedas, "Ajustar Sprite HUDMoedas");
                    imgMoedas.sprite = spWood;
                    imgMoedas.type = UnityEngine.UI.Image.Type.Sliced;
                    imgMoedas.pixelsPerUnitMultiplier = 0.25f;
                    imgMoedas.color = Color.white;
                }

                RectTransform fcMoedas = ObterOuCriarImg(hmRt, "FundoCreme",
                    Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(-16f, -16f), spCream, true, Color.white);
                fcMoedas.SetAsFirstSibling();

                Transform iconeMoeda = hmRt.Find("Image");
                if (iconeMoeda != null)
                {
                    RectTransform imRt = iconeMoeda as RectTransform;
                    imRt.anchorMin = new Vector2(0.5f, 0.5f);
                    imRt.anchorMax = new Vector2(0.5f, 0.5f);
                    imRt.pivot = new Vector2(0.5f, 0.5f);
                    imRt.anchoredPosition = new Vector2(-75f, 0f);
                    imRt.sizeDelta = new Vector2(44f, 44f);
                    imRt.SetAsLastSibling();

                    var img = iconeMoeda.GetComponent<UnityEngine.UI.Image>();
                    if (img != null)
                    {
                        if (spCoin != null) img.sprite = spCoin;
                        img.preserveAspect = true;
                    }
                }

                Transform txtMoedas = hmRt.Find("TextMoedas");
                if (txtMoedas != null)
                {
                    RectTransform tmRt = txtMoedas as RectTransform;
                    tmRt.anchorMin = new Vector2(0.5f, 0.5f);
                    tmRt.anchorMax = new Vector2(0.5f, 0.5f);
                    tmRt.pivot = new Vector2(0f, 0.5f);
                    tmRt.anchoredPosition = new Vector2(-45f, 0f);
                    tmRt.sizeDelta = new Vector2(150f, 48f);
                    tmRt.SetAsLastSibling();

                    var tmp = txtMoedas.GetComponent<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        if (fontPadrao != null) tmp.font = fontPadrao;
                        tmp.text = "Moedas: 0";
                        tmp.fontSize = 34f;
                        tmp.enableAutoSizing = true;
                        tmp.fontSizeMin = 22f;
                        tmp.fontSizeMax = 34f;
                        tmp.fontStyle = FontStyles.Bold;
                        tmp.color = corTextoEscuro;
                        tmp.alignment = TextAlignmentOptions.MidlineLeft;
                    }
                }
            }

            // Botões do Canto Superior Direito (Restart e Pause)
            Transform btnRestart = hudTrans.Find("ButtonRestart") ?? hudTrans.Find("MolduraBotaoRestart/ButtonRestart");
            Transform btnPause = hudTrans.Find("ButtonPause") ?? hudTrans.Find("MolduraBotaoPause/ButtonPause");

            RectTransform molduraRestart = ObterOuCriarImg(hudTrans, "MolduraBotaoRestart",
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-116f, -20f), new Vector2(80f, 80f), spWood, true, Color.white);
            RectTransform fcRes = ObterOuCriarImg(molduraRestart, "FundoCreme",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(-14f, -14f), spCream, true, Color.white);
            fcRes.SetAsFirstSibling();

            RectTransform molduraPause = ObterOuCriarImg(hudTrans, "MolduraBotaoPause",
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-24f, -20f), new Vector2(80f, 80f), spWood, true, Color.white);
            RectTransform fcPau = ObterOuCriarImg(molduraPause, "FundoCreme",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(-14f, -14f), spCream, true, Color.white);
            fcPau.SetAsFirstSibling();

            if (btnRestart != null)
            {
                RectTransform brRt = btnRestart as RectTransform;
                brRt.SetParent(molduraRestart, false);
                brRt.SetAsLastSibling();
                brRt.anchorMin = new Vector2(0.5f, 0.5f);
                brRt.anchorMax = new Vector2(0.5f, 0.5f);
                brRt.pivot = new Vector2(0.5f, 0.5f);
                brRt.anchoredPosition = Vector2.zero;
                brRt.sizeDelta = new Vector2(56f, 56f);
                var img = btnRestart.GetComponent<UnityEngine.UI.Image>();
                if (img != null) img.preserveAspect = true;
            }

            if (btnPause != null)
            {
                RectTransform bpRt = btnPause as RectTransform;
                bpRt.SetParent(molduraPause, false);
                bpRt.SetAsLastSibling();
                bpRt.anchorMin = new Vector2(0.5f, 0.5f);
                bpRt.anchorMax = new Vector2(0.5f, 0.5f);
                bpRt.pivot = new Vector2(0.5f, 0.5f);
                bpRt.anchoredPosition = Vector2.zero;
                bpRt.sizeDelta = new Vector2(56f, 56f);
                var img = btnPause.GetComponent<UnityEngine.UI.Image>();
                if (img != null) img.preserveAspect = true;
            }
        }

        // ==========================================
        // 2. MENU DE PAUSA (PanelPause)
        // ==========================================
        Transform pauseTrans = canvas.transform.Find("PanelPause");
        if (pauseTrans != null)
        {
            RectTransform pRt = pauseTrans as RectTransform;
            pRt.anchorMin = Vector2.zero;
            pRt.anchorMax = Vector2.one;
            pRt.offsetMin = Vector2.zero;
            pRt.offsetMax = Vector2.zero;

            var imgOverlay = pauseTrans.GetComponent<UnityEngine.UI.Image>();
            if (imgOverlay != null)
            {
                imgOverlay.color = new Color(0f, 0f, 0f, 0.74f);
            }

            // Card principal centralizado
            RectTransform cardPause = ObterOuCriarImg(pauseTrans, "CardPause",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(1060f, 640f), spCard, true, Color.white);

            // Faixa de Título
            Transform faixaP = pauseTrans.Find("FaixaTituloPause") ?? cardPause.Find("FaixaTituloPause");
            RectTransform faixaRt = faixaP as RectTransform ?? ObterOuCriarImg(cardPause, "FaixaTituloPause",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -24f), new Vector2(840f, 84f), spHeader, true, Color.white);
            faixaRt.SetParent(cardPause, false);
            faixaRt.anchorMin = new Vector2(0.5f, 1f);
            faixaRt.anchorMax = new Vector2(0.5f, 1f);
            faixaRt.pivot = new Vector2(0.5f, 1f);
            faixaRt.anchoredPosition = new Vector2(0f, -24f);
            faixaRt.sizeDelta = new Vector2(840f, 84f);

            Transform txtTituloAntigo = pauseTrans.Find("JOGO PAUSADO") ?? faixaRt.Find("JOGO PAUSADO") ?? faixaRt.Find("TituloPause");
            if (txtTituloAntigo != null)
            {
                RectTransform tRt = txtTituloAntigo as RectTransform;
                tRt.SetParent(faixaRt, false);
                tRt.name = "TituloPause";
                tRt.anchorMin = Vector2.zero;
                tRt.anchorMax = Vector2.one;
                tRt.pivot = new Vector2(0.5f, 0.5f);
                tRt.offsetMin = new Vector2(20f, 8f);
                tRt.offsetMax = new Vector2(-20f, -8f);

                var tmp = txtTituloAntigo.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    if (fontPadrao != null) tmp.font = fontPadrao;
                    tmp.text = "JOGO PAUSADO";
                    tmp.fontSize = 42f;
                    tmp.enableAutoSizing = true;
                    tmp.fontSizeMin = 26f;
                    tmp.fontSizeMax = 42f;
                    tmp.fontStyle = FontStyles.Bold;
                    tmp.color = corTextoClaro;
                    tmp.alignment = TextAlignmentOptions.Center;
                }
            }

            // Painel central de Áudio (Panel)
            Transform panelAudio = pauseTrans.Find("Panel") ?? cardPause.Find("Panel");
            if (panelAudio != null)
            {
                RectTransform paRt = panelAudio as RectTransform;
                paRt.SetParent(cardPause, false);
                paRt.anchorMin = new Vector2(0.5f, 0.5f);
                paRt.anchorMax = new Vector2(0.5f, 0.5f);
                paRt.pivot = new Vector2(0.5f, 0.5f);
                paRt.anchoredPosition = new Vector2(0f, 25f);
                paRt.sizeDelta = new Vector2(840f, 280f);

                var imgPa = panelAudio.GetComponent<UnityEngine.UI.Image>();
                if (imgPa != null)
                {
                    imgPa.sprite = spWood;
                    imgPa.type = UnityEngine.UI.Image.Type.Sliced;
                    imgPa.pixelsPerUnitMultiplier = 0.25f;
                    imgPa.color = Color.white;
                }

                // FundoInterno DEVE ser o primeiro filho (atrás de Texto e Sliders)
                RectTransform fundoInternoAudio = ObterOuCriarImg(paRt, "FundoInterno",
                    Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(-24f, -24f), spCream, true, Color.white);
                fundoInternoAudio.SetAsFirstSibling();

                // Configurar Textos e Sliders perfeitamente simétricos
                Transform contTexto = paRt.Find("Texto");
                Transform contSliders = paRt.Find("Sliders");

                if (contTexto != null)
                {
                    contTexto.SetAsLastSibling();
                    var vlg = contTexto.GetComponent<VerticalLayoutGroup>();
                    if (vlg != null) vlg.enabled = false;
                    (contTexto as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                    (contTexto as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                    (contTexto as RectTransform).anchoredPosition = Vector2.zero;
                    (contTexto as RectTransform).sizeDelta = new Vector2(840f, 280f);

                    string[] rotulos = { "Volume Geral", "Música", "Efeitos" };
                    string[] nomesObjetosTexto = { "Label_Master", "Label_Musica", "Label_SFX" };
                    float[] posValY = { 72f, 0f, -72f };
                    for (int i = 0; i < contTexto.childCount && i < 3; i++)
                    {
                        var child = contTexto.GetChild(i) as RectTransform;
                        child.name = nomesObjetosTexto[i];
                        child.anchorMin = new Vector2(0.5f, 0.5f);
                        child.anchorMax = new Vector2(0.5f, 0.5f);
                        child.pivot = new Vector2(0f, 0.5f);
                        child.anchoredPosition = new Vector2(-365f, posValY[i]);
                        child.sizeDelta = new Vector2(310f, 48f);

                        var tmp = child.GetComponent<TextMeshProUGUI>();
                        if (tmp != null)
                        {
                            if (fontPadrao != null) tmp.font = fontPadrao;
                            tmp.text = rotulos[i];
                            tmp.fontSize = 32f;
                            tmp.enableAutoSizing = true;
                            tmp.fontSizeMin = 20f;
                            tmp.fontSizeMax = 32f;
                            tmp.fontStyle = FontStyles.Bold;
                            tmp.color = corTextoEscuro;
                            tmp.alignment = TextAlignmentOptions.MidlineLeft;
                        }
                    }
                }

                if (contSliders != null)
                {
                    contSliders.SetAsLastSibling();
                    var vlg = contSliders.GetComponent<VerticalLayoutGroup>();
                    if (vlg != null) vlg.enabled = false;
                    (contSliders as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                    (contSliders as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                    (contSliders as RectTransform).anchoredPosition = Vector2.zero;
                    (contSliders as RectTransform).sizeDelta = new Vector2(840f, 280f);

                    string[] slidersNomes = { "SliderMaster", "SliderMusics", "SliderSFX" };
                    float[] posValY = { 72f, 0f, -72f };
                    for (int i = 0; i < 3; i++)
                    {
                        Transform sl = contSliders.Find(slidersNomes[i]);
                        if (sl != null)
                        {
                            RectTransform slRt = sl as RectTransform;
                            slRt.anchorMin = new Vector2(0.5f, 0.5f);
                            slRt.anchorMax = new Vector2(0.5f, 0.5f);
                            slRt.pivot = new Vector2(0.5f, 0.5f);
                            slRt.anchoredPosition = new Vector2(175f, posValY[i]);
                            slRt.sizeDelta = new Vector2(410f, 42f);
                        }
                    }
                }
            }

            // Botões de Ação Inferiores (Buttons)
            Transform contButtons = pauseTrans.Find("Buttons") ?? cardPause.Find("Buttons");
            if (contButtons != null)
            {
                RectTransform bRt = contButtons as RectTransform;
                bRt.SetParent(cardPause, false);
                bRt.anchorMin = new Vector2(0.5f, 0.5f);
                bRt.anchorMax = new Vector2(0.5f, 0.5f);
                bRt.pivot = new Vector2(0.5f, 0.5f);
                bRt.anchoredPosition = new Vector2(0f, -205f);
                bRt.sizeDelta = new Vector2(840f, 140f);

                var hlg = contButtons.GetComponent<HorizontalLayoutGroup>();
                if (hlg != null) hlg.enabled = false;

                string[] molduras = { "Moldura_ButtonContinuar", "Moldura_ButtonMainMenu", "Moldura_ButtonSairGame" };
                string[] btnNomes = { "ButtonContinuar", "ButtonMainMenu", "ButtonSairGame" };
                string[] labels = { "Continuar", "Ir ao Hub", "Sair" };
                float[] posX = { -280f, 0f, 280f };

                for (int i = 0; i < 3; i++)
                {
                    Transform m = contButtons.Find(molduras[i]);
                    if (m != null)
                    {
                        RectTransform mRt = m as RectTransform;
                        mRt.anchorMin = new Vector2(0.5f, 0.5f);
                        mRt.anchorMax = new Vector2(0.5f, 0.5f);
                        mRt.pivot = new Vector2(0.5f, 0.5f);
                        mRt.anchoredPosition = new Vector2(posX[i], 0f);
                        mRt.sizeDelta = new Vector2(220f, 130f);

                        var imgM = m.GetComponent<UnityEngine.UI.Image>();
                        if (imgM != null)
                        {
                            imgM.sprite = spWood;
                            imgM.type = UnityEngine.UI.Image.Type.Sliced;
                            imgM.pixelsPerUnitMultiplier = 0.25f;
                            imgM.color = Color.white;
                        }

                        RectTransform fcBtn = ObterOuCriarImg(mRt, "FundoInterno",
                            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                            Vector2.zero, new Vector2(-18f, -18f), spCream, true, Color.white);
                        fcBtn.SetAsFirstSibling();

                        Transform btn = m.Find(btnNomes[i]);
                        if (btn != null)
                        {
                            RectTransform btnRt = btn as RectTransform;
                            btnRt.SetAsLastSibling();
                            btnRt.anchorMin = new Vector2(0.5f, 0.5f);
                            btnRt.anchorMax = new Vector2(0.5f, 0.5f);
                            btnRt.pivot = new Vector2(0.5f, 0.5f);
                            btnRt.anchoredPosition = new Vector2(0f, 16f);
                            btnRt.sizeDelta = new Vector2(64f, 64f);
                            var bImg = btn.GetComponent<UnityEngine.UI.Image>();
                            if (bImg != null) bImg.preserveAspect = true;
                        }

                        ObterOuCriarTexto(mRt, "Label",
                            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                            new Vector2(0f, -36f), new Vector2(180f, 34f), labels[i], 24f, corTextoEscuro, TextAlignmentOptions.Center, fontPadrao);
                    }
                }
            }
        }

        // ==========================================
        // 3. TELA DA BALANÇA (PanelBalanca)
        // ==========================================
        Transform balancaTrans = canvas.transform.Find("PanelBalanca");
        if (balancaTrans != null)
        {
            RectTransform pbRt = balancaTrans as RectTransform;
            pbRt.anchorMin = Vector2.zero;
            pbRt.anchorMax = Vector2.one;
            pbRt.offsetMin = Vector2.zero;
            pbRt.offsetMax = Vector2.zero;

            var imgBg = balancaTrans.GetComponent<UnityEngine.UI.Image>();
            if (imgBg != null)
            {
                imgBg.color = new Color(0f, 0f, 0f, 0.78f);
            }

            // Card principal centralizado
            RectTransform cardBalanca = ObterOuCriarImg(balancaTrans, "CardBalanca",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(1140f, 680f), spCard, true, Color.white);

            // Faixa de Título
            RectTransform faixaBalanca = ObterOuCriarImg(cardBalanca, "FaixaTituloBalanca",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -24f), new Vector2(860f, 84f), spHeader, true, Color.white);

            ObterOuCriarTexto(faixaBalanca, "TituloBalanca",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(-40f, -14f), "HORA DA PESAGEM!", 42f, corTextoClaro, TextAlignmentOptions.Center, fontPadrao);

            // Moldura do Visor da Balança (Esquerda)
            RectTransform visorBalanca = ObterOuCriarImg(cardBalanca, "MolduraVisorBalanca",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-200f, -25f), new Vector2(640f, 480f), spWood, true, Color.white);
            RectTransform fcVisor = ObterOuCriarImg(visorBalanca, "FundoCreme",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(-22f, -22f), spCream, true, Color.white);
            fcVisor.SetAsFirstSibling();

            // Mover ImageBalanca para dentro de MolduraVisorBalanca
            Transform imgB = balancaTrans.Find("ImageBalanca") ?? cardBalanca.Find("ImageBalanca") ?? visorBalanca.Find("ImageBalanca");
            if (imgB != null)
            {
                RectTransform ibRt = imgB as RectTransform;
                ibRt.SetParent(visorBalanca, false);
                ibRt.anchorMin = new Vector2(0.5f, 0.5f);
                ibRt.anchorMax = new Vector2(0.5f, 0.5f);
                ibRt.pivot = new Vector2(0.5f, 0.5f);
                ibRt.anchoredPosition = new Vector2(0f, -95f);
                ibRt.sizeDelta = new Vector2(330f, 230f);

                var img = imgB.GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                {
                    if (spBalanca != null) img.sprite = spBalanca;
                    img.preserveAspect = true;
                    img.color = Color.white;
                }
            }

            // Mover LocalFrutas para dentro de MolduraVisorBalanca (sobre o prato da balança)
            Transform localFrutas = balancaTrans.Find("LocalFrutas") ?? cardBalanca.Find("LocalFrutas") ?? visorBalanca.Find("LocalFrutas");
            if (localFrutas != null)
            {
                RectTransform lfRt = localFrutas as RectTransform;
                lfRt.SetParent(visorBalanca, false);
                lfRt.SetAsLastSibling();
                lfRt.anchorMin = new Vector2(0.5f, 0.5f);
                lfRt.anchorMax = new Vector2(0.5f, 0.5f);
                lfRt.pivot = new Vector2(0.5f, 0.5f);
                lfRt.anchoredPosition = new Vector2(0f, 45f);
                lfRt.sizeDelta = new Vector2(320f, 170f);

                var grid = localFrutas.GetComponent<GridLayoutGroup>();
                if (grid == null) grid = localFrutas.gameObject.AddComponent<GridLayoutGroup>();
                grid.cellSize = new Vector2(64f, 64f);
                grid.spacing = new Vector2(6f, 6f);
                grid.childAlignment = TextAnchor.LowerCenter;
                grid.constraint = GridLayoutGroup.Constraint.Flexible;
            }

            // Moldura da Contagem (Direita)
            RectTransform contagemBalanca = ObterOuCriarImg(cardBalanca, "MolduraContagemBalanca",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(325f, -25f), new Vector2(360f, 480f), spWood, true, Color.white);
            RectTransform fcCont = ObterOuCriarImg(contagemBalanca, "FundoCreme",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(-22f, -22f), spCream, true, Color.white);
            fcCont.SetAsFirstSibling();

            ObterOuCriarTexto(contagemBalanca, "TituloContagem",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, 160f), new Vector2(310f, 48f), "ALIMENTOS PESADOS", 24f, corTextoEscuro, TextAlignmentOptions.Center, fontPadrao);

            RectTransform badgeNum = ObterOuCriarImg(contagemBalanca, "BadgeNumero",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, 30f), new Vector2(220f, 170f), spWood, true, Color.white);
            RectTransform fcBadge = ObterOuCriarImg(badgeNum, "FundoInterno",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(-16f, -16f), spCream, true, Color.white);
            fcBadge.SetAsFirstSibling();

            Transform txtContagem = balancaTrans.Find("TextContagem") ?? cardBalanca.Find("TextContagem") ?? badgeNum.Find("TextContagem");
            if (txtContagem != null)
            {
                RectTransform tcRt = txtContagem as RectTransform;
                tcRt.SetParent(badgeNum, false);
                tcRt.SetAsLastSibling();
                tcRt.anchorMin = Vector2.zero;
                tcRt.anchorMax = Vector2.one;
                tcRt.pivot = new Vector2(0.5f, 0.5f);
                tcRt.offsetMin = new Vector2(15f, 10f);
                tcRt.offsetMax = new Vector2(-15f, -10f);

                var tmp = txtContagem.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    if (fontPadrao != null) tmp.font = fontPadrao;
                    tmp.fontSize = 88f;
                    tmp.enableAutoSizing = true;
                    tmp.fontSizeMin = 48f;
                    tmp.fontSizeMax = 88f;
                    tmp.fontStyle = FontStyles.Bold;
                    tmp.color = corMoedaDourada;
                    tmp.alignment = TextAlignmentOptions.Center;
                }
            }

            ObterOuCriarTexto(contagemBalanca, "TextInfo",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, -140f), new Vector2(310f, 44f), "Convertendo em moedas...", 20f, new Color(0.38f, 0.24f, 0.12f, 1f), TextAlignmentOptions.Center, fontPadrao);
        }

        // ==========================================
        // 4. TELA DE GAME OVER (PanelGameOver)
        // ==========================================
        Transform goTrans = canvas.transform.Find("PanelGameOver");
        if (goTrans != null)
        {
            RectTransform gRt = goTrans as RectTransform;
            gRt.anchorMin = Vector2.zero;
            gRt.anchorMax = Vector2.one;
            gRt.offsetMin = Vector2.zero;
            gRt.offsetMax = Vector2.zero;

            var imgBg = goTrans.GetComponent<UnityEngine.UI.Image>();
            if (imgBg != null)
            {
                imgBg.color = new Color(0f, 0f, 0f, 0.82f);
            }

            // Card principal centralizado
            RectTransform cardGo = ObterOuCriarImg(goTrans, "CardGameOver",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(1080f, 650f), spCard, true, Color.white);

            // Faixa de Título
            RectTransform faixaGo = ObterOuCriarImg(cardGo, "FaixaTituloGameOver",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -24f), new Vector2(860f, 84f), spHeader, true, Color.white);

            ObterOuCriarTexto(faixaGo, "TituloGameOver",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(-40f, -14f), "CORRIDA CONCLUÍDA!", 42f, corTextoClaro, TextAlignmentOptions.Center, fontPadrao);

            // Moldura do Resumo de Score
            RectTransform resumoScore = ObterOuCriarImg(cardGo, "MolduraResumoScore",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, 105f), new Vector2(880f, 160f), spWood, true, Color.white);
            RectTransform fcScore = ObterOuCriarImg(resumoScore, "FundoCreme",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(-22f, -22f), spCream, true, Color.white);
            fcScore.SetAsFirstSibling();

            Transform txtScore = goTrans.Find("TextScore") ?? cardGo.Find("TextScore") ?? resumoScore.Find("TextScore");
            if (txtScore != null)
            {
                RectTransform tsRt = txtScore as RectTransform;
                tsRt.SetParent(resumoScore, false);
                tsRt.SetAsLastSibling();
                tsRt.anchorMin = Vector2.zero;
                tsRt.anchorMax = Vector2.one;
                tsRt.pivot = new Vector2(0.5f, 0.5f);
                tsRt.offsetMin = new Vector2(25f, 15f);
                tsRt.offsetMax = new Vector2(-25f, -15f);

                var tmp = txtScore.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    if (fontPadrao != null) tmp.font = fontPadrao;
                    tmp.fontSize = 34f;
                    tmp.enableAutoSizing = true;
                    tmp.fontSizeMin = 22f;
                    tmp.fontSizeMax = 34f;
                    tmp.fontStyle = FontStyles.Bold;
                    tmp.color = corTextoEscuro;
                    tmp.alignment = TextAlignmentOptions.Center;
                }
            }

            // Moldura do Bônus de Moedas
            RectTransform bonusMoedas = ObterOuCriarImg(cardGo, "MolduraBonusMoedas",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, -35f), new Vector2(640f, 84f), spWood, true, Color.white);
            RectTransform fcBonus = ObterOuCriarImg(bonusMoedas, "FundoCreme",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(-18f, -18f), spCream, true, Color.white);
            fcBonus.SetAsFirstSibling();

            RectTransform iconeBonus = ObterOuCriarImg(bonusMoedas, "IconeMoedaBonus",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-190f, 0f), new Vector2(46f, 46f), spCoin, false, Color.white);
            iconeBonus.SetAsLastSibling();

            Transform txtBonus = goTrans.Find("TextMoedasBonus") ?? cardGo.Find("TextMoedasBonus") ?? bonusMoedas.Find("TextMoedasBonus");
            if (txtBonus != null)
            {
                RectTransform tbRt = txtBonus as RectTransform;
                tbRt.SetParent(bonusMoedas, false);
                tbRt.SetAsLastSibling();
                tbRt.anchorMin = new Vector2(0.5f, 0.5f);
                tbRt.anchorMax = new Vector2(0.5f, 0.5f);
                tbRt.pivot = new Vector2(0f, 0.5f);
                tbRt.anchoredPosition = new Vector2(-155f, 0f);
                tbRt.sizeDelta = new Vector2(380f, 50f);

                var tmp = txtBonus.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    if (fontPadrao != null) tmp.font = fontPadrao;
                    tmp.fontSize = 36f;
                    tmp.enableAutoSizing = true;
                    tmp.fontSizeMin = 22f;
                    tmp.fontSizeMax = 36f;
                    tmp.fontStyle = FontStyles.Bold;
                    tmp.color = corMoedaDourada;
                    tmp.alignment = TextAlignmentOptions.Center;
                }
            }

            // Botões de Ação Inferiores (HorizontalAlign)
            Transform contGoButtons = goTrans.Find("HorizontalAlign") ?? cardGo.Find("HorizontalAlign");
            if (contGoButtons != null)
            {
                RectTransform cbRt = contGoButtons as RectTransform;
                cbRt.SetParent(cardGo, false);
                cbRt.anchorMin = new Vector2(0.5f, 0.5f);
                cbRt.anchorMax = new Vector2(0.5f, 0.5f);
                cbRt.pivot = new Vector2(0.5f, 0.5f);
                cbRt.anchoredPosition = new Vector2(0f, -205f);
                cbRt.sizeDelta = new Vector2(860f, 140f);

                var hlg = contGoButtons.GetComponent<HorizontalLayoutGroup>();
                if (hlg != null) hlg.enabled = false;

                string[] moldurasGo = { "Moldura_RestartGO", "Moldura_MenuGO", "Moldura_ExitGO" };
                string[] btnNomesGo = { "ButtonRestartGameOver", "ButtonMainMenuGameOver", "ButtonExitGameOver" };
                string[] labelsGo = { "Jogar de Novo", "Ir ao Hub", "Sair" };
                float[] posXGo = { -280f, 0f, 280f };

                for (int i = 0; i < 3; i++)
                {
                    RectTransform m = ObterOuCriarImg(cbRt, moldurasGo[i],
                        new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                        new Vector2(posXGo[i], 0f), new Vector2(230f, 135f), spWood, true, Color.white);

                    RectTransform fcBtnGo = ObterOuCriarImg(m, "FundoInterno",
                        Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                        Vector2.zero, new Vector2(-18f, -18f), spCream, true, Color.white);
                    fcBtnGo.SetAsFirstSibling();

                    Transform btn = cbRt.Find(btnNomesGo[i]);
                    if (btn != null)
                    {
                        RectTransform btnRt = btn as RectTransform;
                        btnRt.SetParent(m, false);
                        btnRt.SetAsLastSibling();
                        btnRt.anchorMin = new Vector2(0.5f, 0.5f);
                        btnRt.anchorMax = new Vector2(0.5f, 0.5f);
                        btnRt.pivot = new Vector2(0.5f, 0.5f);
                        btnRt.anchoredPosition = new Vector2(0f, 16f);
                        btnRt.sizeDelta = new Vector2(64f, 64f);
                        var bImg = btn.GetComponent<UnityEngine.UI.Image>();
                        if (bImg != null) bImg.preserveAspect = true;
                    }

                    ObterOuCriarTexto(m, "Label",
                        new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                        new Vector2(0f, -36f), new Vector2(190f, 34f), labelsGo[i], 24f, corTextoEscuro, TextAlignmentOptions.Center, fontPadrao);
                }
            }
        }

        // ==========================================
        // 5. ATUALIZAR REFERÊNCIAS SERIALIZADAS DO GAMEMANAGER
        // ==========================================
        if (gm != null)
        {
            Undo.RecordObject(gm, "Atualizar referências GameManager");
            if (hudTrans != null)
            {
                gm.hudDoJogo = hudTrans.gameObject;
                var ta = hudTrans.Find("MolduraColetadosHUD/TextHUDAlimentosTotal") ?? hudTrans.Find("TextHUDAlimentosTotal");
                if (ta != null) gm.textoHUDTotal = ta.GetComponent<TextMeshProUGUI>();

                var tm = hudTrans.Find("HUDMoedas/TextMoedas");
                if (tm != null) gm.textoHUDMoedas = tm.GetComponent<TextMeshProUGUI>();
            }

            if (balancaTrans != null)
            {
                gm.painelBalanca = balancaTrans.gameObject;
                var lf = balancaTrans.Find("CardBalanca/MolduraVisorBalanca/LocalFrutas") ?? balancaTrans.Find("LocalFrutas");
                if (lf != null) gm.localDasFrutasNaBalanca = lf;

                var tc = balancaTrans.Find("CardBalanca/MolduraContagemBalanca/BadgeNumero/TextContagem") ?? balancaTrans.Find("TextContagem");
                if (tc != null) gm.textoBalancaContagem = tc.GetComponent<TextMeshProUGUI>();
            }

            if (goTrans != null)
            {
                gm.gameOverPanel = goTrans.gameObject;
                var ts = goTrans.Find("CardGameOver/MolduraResumoScore/TextScore") ?? goTrans.Find("TextScore");
                if (ts != null) gm.textoPontuacaoFinal = ts.GetComponent<TextMeshProUGUI>();

                var tb = goTrans.Find("CardGameOver/MolduraBonusMoedas/TextMoedasBonus") ?? goTrans.Find("TextMoedasBonus");
                if (tb != null) gm.textoMoedasGanhas = tb.GetComponent<TextMeshProUGUI>();
            }

            EditorUtility.SetDirty(gm);
        }
    }

    private static RectTransform ObterOuCriarImg(Transform pai, string nome, Vector2 ancMin, Vector2 ancMax, Vector2 piv, Vector2 pos, Vector2 size, Sprite sp, bool sliced, Color cor)
    {
        Transform t = pai.Find(nome);
        GameObject go = t != null ? t.gameObject : new GameObject(nome, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
        if (t == null)
        {
            Undo.RegisterCreatedObjectUndo(go, "Criar " + nome);
            go.transform.SetParent(pai, false);
        }
        else
        {
            Undo.RecordObject(go.transform, "Ajustar " + nome);
        }

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = ancMin;
        rt.anchorMax = ancMax;
        rt.pivot = piv;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        rt.localScale = Vector3.one;

        var img = go.GetComponent<UnityEngine.UI.Image>();
        Undo.RecordObject(img, "Ajustar Imagem " + nome);
        if (sp != null) img.sprite = sp;
        img.type = sliced ? UnityEngine.UI.Image.Type.Sliced : UnityEngine.UI.Image.Type.Simple;
        img.pixelsPerUnitMultiplier = sliced ? 0.25f : 1f;
        img.preserveAspect = !sliced;
        img.color = cor;
        img.raycastTarget = false;
        return rt;
    }

    private static TextMeshProUGUI ObterOuCriarTexto(Transform pai, string nome, Vector2 ancMin, Vector2 ancMax, Vector2 piv, Vector2 pos, Vector2 size, string texto, float tamMax, Color cor, TextAlignmentOptions align, TMP_FontAsset font)
    {
        Transform t = pai.Find(nome);
        GameObject go = t != null ? t.gameObject : new GameObject(nome, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        if (t == null)
        {
            Undo.RegisterCreatedObjectUndo(go, "Criar " + nome);
            go.transform.SetParent(pai, false);
        }
        else
        {
            Undo.RecordObject(go.transform, "Ajustar " + nome);
        }

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = ancMin;
        rt.anchorMax = ancMax;
        rt.pivot = piv;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        rt.localScale = Vector3.one;

        var tmp = go.GetComponent<TextMeshProUGUI>();
        Undo.RecordObject(tmp, "Ajustar Texto " + nome);
        if (font != null) tmp.font = font;
        tmp.text = texto;
        tmp.fontSize = tamMax;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = Mathf.Round(tamMax * 0.55f);
        tmp.fontSizeMax = tamMax;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = cor;
        tmp.alignment = align;
        tmp.raycastTarget = false;
        return tmp;
    }
}
