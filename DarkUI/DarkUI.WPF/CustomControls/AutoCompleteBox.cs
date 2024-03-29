﻿using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace DarkUI.WPF.CustomControls;

[TemplatePart(Name = "PART_InputTextBox", Type = typeof(TextBox))]
[TemplatePart(Name = "PART_SuggestionBox", Type = typeof(ListBox))]
public class AutoCompleteBox : Control
{
	public static readonly DependencyProperty TextProperty;
	public static readonly DependencyProperty IsSuggestionVisibleProperty;
	public static readonly DependencyProperty ItemsSourceProperty;
	public static readonly DependencyProperty SelectItemProperty;

	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public bool IsSuggestionVisible
	{
		get => (bool)GetValue(IsSuggestionVisibleProperty);
		set => SetValue(IsSuggestionVisibleProperty, value);
	}

	public IEnumerable ItemsSource
	{
		get => (IEnumerable)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	public ICommand SelectItem
	{
		get => (ICommand)GetValue(SelectItemProperty);
		set => SetValue(SelectItemProperty, value);
	}

	public TextBox InputTextBox { get; set; } = null!;
	public Popup Popup { get; set; } = null!;
	public ListBox SuggestionBox { get; set; } = null!;

	static AutoCompleteBox()
	{
		TextProperty = DependencyProperty.Register(
			nameof(Text),
			typeof(string),
			typeof(AutoCompleteBox),
			new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		IsSuggestionVisibleProperty = DependencyProperty.Register(
			nameof(IsSuggestionVisible),
			typeof(bool),
			typeof(AutoCompleteBox),
			new PropertyMetadata(false));

		ItemsSourceProperty = DependencyProperty.Register(
			nameof(ItemsSource),
			typeof(IEnumerable),
			typeof(AutoCompleteBox),
			new PropertyMetadata(null));

		SelectItemProperty = DependencyProperty.Register(
			nameof(SelectItem),
			typeof(ICommand),
			typeof(AutoCompleteBox),
			new PropertyMetadata(null));
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		InputTextBox = (TextBox)GetTemplateChild("PART_InputTextBox");
		InputTextBox.TextChanged += UpdateSuggestion;
		InputTextBox.PreviewKeyDown += InputTextBox_PreviewKeyDown;
		InputTextBox.LostFocus += OnLostFocus;

		Popup = (Popup)GetTemplateChild("PART_Popup");
		Popup.Closed += (sender, e) => IsSuggestionVisible = false;

		SuggestionBox = (ListBox)GetTemplateChild("PART_SuggestionBox");
		SuggestionBox.PreviewKeyDown += SuggestionBox_PreviewKeyDown;
		SuggestionBox.MouseDoubleClick += (sender, e) => SelectItem.Execute(SuggestionBox.SelectedItem);
		SuggestionBox.LostFocus += OnLostFocus;

		SelectItem = new RelayCommand(obj =>
		{
			string? text = obj?.ToString();

			if (string.IsNullOrEmpty(text))
				return;

			int lastSpaceIndex = InputTextBox.Text.LastIndexOf(' ');
			lastSpaceIndex = lastSpaceIndex == -1 ? 0 : lastSpaceIndex + 1;

			InputTextBox.Text = InputTextBox.Text[..lastSpaceIndex] + text;
			InputTextBox.CaretIndex = InputTextBox.Text.Length;
			InputTextBox.Focus();

			IsSuggestionVisible = false;
		});
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		if (e.Property == TextProperty)
			InputTextBox.Text = (string)e.NewValue;

		base.OnPropertyChanged(e);
	}

	private void OnLostFocus(object sender, RoutedEventArgs e)
	{
		if (!IsSuggestionVisible)
			Text = InputTextBox.Text;
	}

	private void InputTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Down && IsSuggestionVisible && InputTextBox.IsFocused)
		{
			var container = (ListBoxItem)SuggestionBox.ItemContainerGenerator.ContainerFromIndex(0);
			Dispatcher.BeginInvoke(() => container.Focus(), DispatcherPriority.Normal);
			SuggestionBox.SelectedIndex = 0;
		}
	}

	private void SuggestionBox_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Up && SuggestionBox.SelectedIndex == 0 && !InputTextBox.IsFocused)
		{
			InputTextBox.Focus();
			InputTextBox.CaretIndex = InputTextBox.Text.Length;

			SuggestionBox.SelectedIndex = -1;
		}
	}

	private void UpdateSuggestion(object sender, TextChangedEventArgs e)
	{
		SuggestionBox.SelectedIndex = -1;

		string lastItem = InputTextBox.Text.Split(' ').Last();

		if (string.IsNullOrWhiteSpace(lastItem))
		{
			IsSuggestionVisible = false;
			return;
		}

		SuggestionBox.Items.Filter = obj =>
		{
			string? text = obj?.ToString();

			if (string.IsNullOrEmpty(text))
				return false;

			return text.Contains(lastItem, StringComparison.OrdinalIgnoreCase);
		};

		IsSuggestionVisible = InputTextBox.IsKeyboardFocusWithin && !string.IsNullOrWhiteSpace(InputTextBox.Text);
	}
}
