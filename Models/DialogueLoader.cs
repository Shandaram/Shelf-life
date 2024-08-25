using UnityEngine;
using System.Collections.Generic;
using LitJson;

[System.Serializable]
public class DialogueTree
{
    public Dictionary<string, DialogueNode> nodes;
}

[System.Serializable]
public class DialogueNode
{
    public string content;
    public Dictionary<string, Choice> choices;
}

[System.Serializable]
public class Choice
{
    public string text;
    public string next;
    public string tag; // Default empty tag
    public int pep = 0;    // Default to 0 if not provided
    public int rep = 0;    // Default to 0 if not provided
    public int coins = 0;  // Default to 0 if not provided
}

public class DialogueLoader : MonoBehaviour
{
    public DialogueTree dialogueTree;

    public delegate void DialogueDataLoaded();
    public event DialogueDataLoaded OnDialogueDataLoaded;

    public void LoadDialogueData(TextAsset dialogueJsonFile)
    {
        if (dialogueJsonFile != null)
        {
            string jsonData = dialogueJsonFile.text;

            try
            {
                JsonData json = JsonMapper.ToObject(jsonData);
                dialogueTree = ParseDialogueTree(json);

                if (dialogueTree != null && dialogueTree.nodes != null)
                {
                    Debug.Log("Dialogue tree loaded.");
                }
                else
                {
                    Debug.LogError("Dialogue data or nodes are null.");
                }

                OnDialogueDataLoaded?.Invoke();
            }
            catch (JsonException e)
            {
                Debug.LogError($"JSON Parsing Error: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("Dialogue JSON file is not assigned.");
        }
    }

    private DialogueTree ParseDialogueTree(JsonData json)
    {
        var dialogueTree = new DialogueTree
        {
            nodes = new Dictionary<string, DialogueNode>()
        };

        foreach (string key in json.Keys)
        {
            JsonData nodeData = json[key];
            var node = new DialogueNode
            {
                content = (string)nodeData["content"],
                choices = new Dictionary<string, Choice>()
            };

            foreach (string choiceKey in nodeData["choices"].Keys)
            {
                JsonData choiceData = nodeData["choices"][choiceKey];
                var choice = new Choice
                {
                    text = (string)choiceData["text"],
                    next = (string)choiceData["next"],
                    tag = choiceData.Keys.Contains("tag") ? (string)choiceData["tag"] : "", // Handle optional tag
                    pep = choiceData.Keys.Contains("pep") ? (int)choiceData["pep"] : 0,   // Default to 0 if missing
                    rep = choiceData.Keys.Contains("rep") ? (int)choiceData["rep"] : 0,   // Default to 0 if missing
                    coins = choiceData.Keys.Contains("coins") ? (int)choiceData["coins"] : 0 // Default to 0 if missing
                };
                node.choices.Add(choiceKey, choice);
            }

            dialogueTree.nodes.Add(key, node);
        }

        return dialogueTree;
    }

    public DialogueTree GetDialogueData()
    {
        return dialogueTree;
    }

     public string GetDialogueContent(string nodeKey)
    {
        // Check if dialogueTree and its nodes are not null
        if (dialogueTree != null && dialogueTree.nodes != null)
        {
            // Check if the nodeKey exists in the nodes dictionary
            if (dialogueTree.nodes.TryGetValue(nodeKey, out DialogueNode node))
            {
                // Return the content of the node
                return node.content;
            }
            else
            {
                Debug.LogWarning($"Node with key '{nodeKey}' not found.");
                return "Node not found.";
            }
        }
        else
        {
            Debug.LogError("Dialogue tree or nodes are not loaded.");
            return "Dialogue tree not loaded.";
        }
    }
}
