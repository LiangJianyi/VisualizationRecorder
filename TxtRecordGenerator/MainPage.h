#pragma once

#include "MainPage.g.h"
#include "../../SundryUtilty/Cpp code files/JanyeeDateTime/DateTime.h"

using namespace winrt;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;

namespace winrt::TxtRecordGenerator::implementation
{
    struct MainPage : MainPageT<MainPage>
    {
        MainPage();
		void UpdateLayout();
		IAsyncAction GeneratorButton_ClickAsync(IInspectable const& sender, RoutedEventArgs const& e);
        IAsyncAction GenerateDateEntryAsync(StorageFolder folder, std::shared_ptr<std::wstringstream> s, Janyee::DateTime beginDateTime, Janyee::DateTime endDateTime);
	};
}

namespace winrt::TxtRecordGenerator::factory_implementation
{
    struct MainPage : MainPageT<MainPage, implementation::MainPage>
    {
    };
}
