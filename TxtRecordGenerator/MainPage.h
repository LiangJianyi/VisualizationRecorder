#pragma once

#include "MainPage.g.h"

namespace winrt::TxtRecordGenerator::implementation
{
    struct MainPage : MainPageT<MainPage>
    {
        MainPage();
		void UpdateLayout();
		void GeneratorButton_Click(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::UI::Xaml::RoutedEventArgs const& e);
		void BeginDatePicker_Opened(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::Foundation::IInspectable const& e);
		void BeginDatePicker_DateChanged(winrt::Windows::UI::Xaml::Controls::CalendarDatePicker const& sender, winrt::Windows::UI::Xaml::Controls::CalendarDatePickerDateChangedEventArgs const& args);
		void BeginDatePicker_Closed(winrt::Windows::Foundation::IInspectable const& sender, winrt::Windows::Foundation::IInspectable const& e);
		void BeginDatePicker_CalendarViewDayItemChanging(winrt::Windows::UI::Xaml::Controls::CalendarView const& sender, winrt::Windows::UI::Xaml::Controls::CalendarViewDayItemChangingEventArgs const& e);
	};
}

namespace winrt::TxtRecordGenerator::factory_implementation
{
    struct MainPage : MainPageT<MainPage, implementation::MainPage>
    {
    };
}
