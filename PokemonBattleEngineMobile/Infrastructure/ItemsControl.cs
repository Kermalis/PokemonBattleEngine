//
// ItemsControl.cs
//
// Author:
//       Mark Smith <smmark@microsoft.com>
//
// Copyright (c) 2016-2018 Xamarin, Microsoft.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Infrastructure
{
    /// <summary>
    /// Simple ItemsControl to render a list of things in a stacked view using
    /// either text labels, or an inflated data template. It also includes the ability
    /// to display a text placeholder if no items are present in the data bound collection.
    /// </summary>
    public class ItemsControl : ContentView
    {
        /// <summary>
        /// Bindable property for the placeholder text.
        /// </summary>
        public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(
            nameof(PlaceholderText), typeof(string), typeof(ItemsControl));

        /// <summary>
        /// Gets or sets the placeholder text.
        /// </summary>
        /// <value>The placeholder text.</value>
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        /// <summary>
        /// Bindable property for the orientation of the default layout panel
        /// </summary>
        public static readonly BindableProperty OrientationProperty = BindableProperty.Create(
            nameof(Orientation), typeof(StackOrientation), typeof(ItemsControl),
            defaultValue: StackOrientation.Vertical,
            propertyChanged: (bindable, oldValue, newValue) => ((ItemsControl)bindable).OnOrientationPropertyChanged((StackOrientation)oldValue, (StackOrientation)newValue));

        /// <summary>
        /// Gets or Sets the Orientation for the default layout panel
        /// This is not used if you replace the panel!
        /// </summary>
        /// <value>Orientation value</value>
        public StackOrientation Orientation
        {
            get { return (StackOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Bindable property for the Spacing of the default layout panel
        /// </summary>
        public static readonly BindableProperty SpacingProperty = BindableProperty.Create(
            nameof(Spacing), typeof(double), typeof(ItemsControl),
            defaultValue: 10.0,
            propertyChanged: (bindable, oldValue, newValue) => ((ItemsControl)bindable).OnSpacingPropertyChanged((double)oldValue, (double)newValue));

        /// <summary>
        /// Gets or Sets the Spacing for the default layout panel
        /// This is not used if you replace the panel!
        /// </summary>
        /// <value>Spacing value</value>
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        /// <summary>
        /// Bindable property for the Label style used for each item when there
        /// is no data template assigned.
        /// </summary>
        public static readonly BindableProperty ItemStyleProperty = BindableProperty.Create(
            nameof(ItemStyle), typeof(Style), typeof(ItemsControl),
            propertyChanged: (bindable, oldValue, newValue) => ((ItemsControl)bindable).OnItemStylePropertyChanged(newValue as Style));

        /// <summary>
        /// Gets or sets the item style used for dynamically generated labels.
        /// This is not used if you apply a DataTemplate
        /// </summary>
        /// <value>The item style.</value>
        public Style ItemStyle
        {
            get { return (Style)GetValue(ItemStyleProperty); }
            set { SetValue(ItemStyleProperty, value); }
        }

        /// <summary>
        /// Bindable property for the panel type
        /// </summary>
        public static readonly BindableProperty ItemsPanelProperty = BindableProperty.Create(
            nameof(ItemsPanel), typeof(Xamarin.Forms.Layout<View>), typeof(ItemsControl),
            propertyChanged: (bindable, oldValue, newValue) => ((ItemsControl)bindable).OnItemsPanelPropertyChanged((Xamarin.Forms.Layout<View>)oldValue, (Xamarin.Forms.Layout<View>)newValue));

        /// <summary>
        /// Gets or Sets the container used for the layout panel
        /// If not set, a StackLayout is used.
        /// </summary>
        /// <value>Orientation value</value>
        public Xamarin.Forms.Layout<View> ItemsPanel
        {
            get { return (Xamarin.Forms.Layout<View>)GetValue(ItemsPanelProperty); }
            set { SetValue(ItemsPanelProperty, value); }
        }

        /// <summary>
        /// Bindable property for the data source
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource), typeof(IList), typeof(ItemsControl),
            propertyChanging: (bindable, oldValue, newValue) => ((ItemsControl)bindable).OnItemsSourceChanged((IList)oldValue, (IList)newValue));

        /// <summary>
        /// Gets or sets the items source - can be any collection of elements.
        /// </summary>
        /// <value>The items source.</value>
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Bindable property for the data template to visually represent each item.
        /// </summary>
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate), typeof(DataTemplate), typeof(ItemsControl),
            propertyChanging: (bindable, oldValue, newValue) => ((ItemsControl)bindable).OnItemTemplateChanged((DataTemplate)oldValue, (DataTemplate)newValue));

        /// <summary>
        /// Gets or sets the item template used to generate the visuals for a single item.
        /// </summary>
        /// <value>The item template.</value>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Data
        Label noItemsLabel;
        StackLayout stack;

        /// <summary>
        /// Initializes an ItemsControl.
        /// </summary>
        public ItemsControl()
        {
            Padding = new Thickness(5, 0, 5, 5);

            noItemsLabel = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            noItemsLabel.SetBinding(Label.StyleProperty, new Binding(nameof(ItemStyle), source: this));
            noItemsLabel.SetBinding(Label.TextProperty, new Binding(nameof(PlaceholderText), source: this));

            Content = noItemsLabel;
        }

        /// <summary>
        /// Retrieve or create the container for children
        /// </summary>
        /// <param name="createDefaultContainer">True to create default container if ItemsPanel is not set</param>
        /// <returns>Layout container</returns>
        private Xamarin.Forms.Layout<View> GetOrCreateLayoutContainer(bool createDefaultContainer)
        {
            if (ItemsPanel != null)
            {
                return ItemsPanel;
            }

            if (createDefaultContainer && stack == null)
            {
                stack = new StackLayout
                {
                    Orientation = this.Orientation,
                    Spacing = this.Spacing,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };
            }

            return stack;
        }

        /// <summary>
        /// Instance method used to change the current orientation of the layout panel.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnOrientationPropertyChanged(StackOrientation oldValue, StackOrientation newValue)
        {
            if (stack != null && oldValue != newValue)
            {
                stack.Orientation = newValue;
            }
        }

        /// <summary>
        /// Instance method used to change the current spacing of the layout panel.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnSpacingPropertyChanged(double oldValue, double newValue)
        {
            if (stack != null && oldValue != newValue)
            {
                stack.Spacing = newValue;
            }
        }

        /// <summary>
        /// This is called when the ItemsPanel property is changed. We need to clear the old object and
        /// fill our data into the new one.
        /// </summary>
        /// <param name="oldValue">Old panel</param>
        /// <param name="newValue">New panel</param>
        private void OnItemsPanelPropertyChanged(Xamarin.Forms.Layout<View> oldValue, Xamarin.Forms.Layout<View> newValue)
        {
            this.Content = null; // temporarily show nothing.

            if (stack != null)
            {
                stack.Children.Clear();
                if (newValue != null)
                {
                    stack = null;
                }
            }

            if (oldValue != null)
            {
                oldValue.Children.Clear();
            }

            // Will recreate the container if necessary
            FillContainer(newValue, ItemsSource, ItemTemplate);
        }

        /// <summary>
        /// Instance method used to change the ItemTemplate for each rendered item.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnItemTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }

            var data = this.ItemsSource;
            if (data?.Count > 0)
            {
                // Remove all the generated visuals
                var container = GetOrCreateLayoutContainer(true);
                container.Children.Clear();

                // Regenerate
                FillContainer(container, data, newValue);
            }
        }

        /// <summary>
        /// Instance method called when the underlying data source is changed through the
        /// <see cref="ItemsSource"/> property. This re-generates the list based on the 
        /// new collection.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        void OnItemsSourceChanged(IList oldValue, IList newValue)
        {
            // Unsubscribe from the old collection
            if (oldValue != null)
            {
                INotifyCollectionChanged ncc = oldValue as INotifyCollectionChanged;
                if (ncc != null)
                {
                    ncc.CollectionChanged -= OnCollectionChanged;
                }
            }

            if (newValue == null)
            {
                var container = GetOrCreateLayoutContainer(false);
                if (container != null)
                {
                    container.Children.Clear();
                    stack = null;
                }

                Content = noItemsLabel;
            }
            else
            {
                FillContainer(null, newValue, ItemTemplate);
                INotifyCollectionChanged ncc = newValue as INotifyCollectionChanged;
                if (ncc != null)
                {
                    ncc.CollectionChanged += OnCollectionChanged;
                }
            }
        }

        /// <summary>
        /// Instance method called when the label style is changed through the
        /// <see cref="ItemStyle"/> property. This applies the new style to all the labels.
        /// </summary>
        /// <param name="style">Style.</param>
        void OnItemStylePropertyChanged(Style style)
        {
            // Ignore if we have a data template.
            if (ItemTemplate != null)
            {
                return;
            }

            var container = GetOrCreateLayoutContainer(false);
            if (container != null)
            {
                foreach (View view in container.Children)
                {
                    Label label = view as Label;
                    if (label != null)
                    {

                        if (style == null)
                        {
                            label.ClearValue(Label.StyleProperty);
                        }
                        else
                        {
                            label.Style = style;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method takes our items source and generates visuals for
        /// each item in the collection; it can reuse visuals which were created
        /// previously and simply changes the binding context.
        /// </summary>
        /// <param name="container">Visual container to add items to</param>
        /// <param name="newValue">New items to display</param>
        /// <param name="itemTemplate">ItemTemplate to use (null for Label)</param>
        void FillContainer(Xamarin.Forms.Layout<View> container, IList newValue, DataTemplate itemTemplate)
        {
            if (container == null)
            {
                container = GetOrCreateLayoutContainer(true);
            }

            // No items? Show the "no content" label.
            if (newValue == null || newValue.Count == 0)
            {
                Content = noItemsLabel;
                container.Children.Clear();
                stack = null;
                return;
            }

            // Add items
            var itemStyle = ItemStyle;
            var visuals = container.Children;

            for (int i = 0; i < visuals.Count; i++)
            {
                visuals[i].IsVisible = i < newValue.Count;
            }

            for (int i = 0; i < newValue.Count; i++)
            {
                var dataItem = newValue[i];

                if (visuals.Count > i)
                {
                    if (itemTemplate != null)
                    {
                        var visualItem = visuals[i];
                        visualItem.BindingContext = dataItem;
                    }
                    else
                    {
                        Label visualItem = (Label)visuals[i];
                        visualItem.Text = dataItem.ToString();
                        if (itemStyle != null)
                        {
                            visualItem.Style = itemStyle;
                        }
                        else
                        {
                            visualItem.ClearValue(Label.StyleProperty);
                        }
                    }
                }
                else
                {
                    if (itemTemplate != null)
                    {
                        var view = InflateTemplate(itemTemplate, dataItem);
                        container.Children.Add(view);
                    }
                    else
                    {
                        var label = new Label { Text = dataItem.ToString() };
                        if (itemStyle != null)
                        {
                            label.Style = itemStyle;
                        }
                        container.Children.Add(label);
                    }
                }
            }

            Content = container;
        }

        /// <summary>
        /// Inflates the visuals for a data template or template selector
        /// and adds it to our StackLayout.
        /// </summary>
        /// <param name="template">Template.</param>
        /// <param name="item">Item.</param>
        View InflateTemplate(DataTemplate template, object item)
        {
            // Pull real template from selector if necessary.
            var dSelector = template as DataTemplateSelector;
            if (dSelector != null)
            {
                template = dSelector.SelectTemplate(item, this);
            }

            var view = template.CreateContent() as View;
            if (view != null)
            {
                view.BindingContext = item;
            }

            return view;
        }

        /// <summary>
        /// This is called when the data source collection implements
        /// collection change notifications and the data has changed.
        /// This is not optimized - it simply replaces all the data.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FillContainer(null, (IList)sender, ItemTemplate);
        }
    }
}