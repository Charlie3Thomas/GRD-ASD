#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CT.Components
{
    using Data.Save;
    using Enums;
    using Utils;
    using GraphView;

    public class CTChoiceNode : CTNode
    {
        public override void Initialise(string _node_name, CTGraphView _graph_view, Vector2 _pos)
        {
            base.Initialise(_node_name, _graph_view, _pos);

            node_type = CTNodeType.Choice;

            CTOptionSaveData option_data = new CTOptionSaveData();
            option_data.text = "New Option";

            options.Add(option_data);
        }

        public override void Draw()
        {
            base.Draw();

            Button add_option = CTComponentUtility.CreateButton("Add Option", () =>
            {
                CTOptionSaveData option_data = new CTOptionSaveData();
                option_data.text = "New Option";

                options.Add(option_data);

                Port p_out = CreateOutputPort(option_data);

                outputContainer.Add(p_out);
            });

            mainContainer.Insert(1, add_option);

            foreach (CTOptionSaveData o in options)
            {
                Port p_out = CreateOutputPort(o);

                outputContainer.Add(p_out);
            }

            RefreshExpandedState();
        }


        #region Utility
        private Port CreateOutputPort(object _data)
        {
            Port p_out = this.CreatePort();

            p_out.userData = _data;

            CTOptionSaveData option = (CTOptionSaveData)_data;

            Button remove_option = CTComponentUtility.CreateButton("Remove", () =>
            {
                if (options.Count == 1)
                    return;

                if (p_out.connected)
                    graph_view.DeleteElements(p_out.connections);

                options.Remove(option);

                graph_view.RemoveElement(p_out);
            });

            TextField option_text = CTComponentUtility.CreateTextField(option.text, null, callback =>
            {
                option.text = callback.newValue;
            });

            p_out.Add(option_text);
            p_out.Add(remove_option);

            return p_out;
        }

        #endregion
    }
}
#endif