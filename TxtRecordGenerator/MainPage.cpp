#include "pch.h"
#include "MainPage.h"
#include "MainPage.g.cpp"
#include "../../SundryUtilty/Cpp code files/JanyeeDateTime/DateTime.h"
#include "MainPageHelper.h"
#include <string>

using namespace winrt;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;

namespace winrt::TxtRecordGenerator::implementation
{
	MainPage::MainPage() {
		InitializeComponent();
		UpdateLayout();
	}

	void MainPage::UpdateLayout() {
		OutputDebugStringA((std::string("GeneratorButton().ActualWidth(): ") + std::to_string(GeneratorButton().ActualWidth()) + "\n").c_str());
		OutputDebugStringA((std::string("EntriesTotal().ActualWidth(): ") + std::to_string(EntriesTotal().ActualWidth()) + "\n").c_str());
		//EntriesTotal().Width(GeneratorButton().ActualWidth());
	}
}


IAsyncAction winrt::TxtRecordGenerator::implementation::MainPage::GeneratorButton_ClickAsync(winrt::Windows::Foundation::IInspectable const&, winrt::Windows::UI::Xaml::RoutedEventArgs const&) {
	IReference<DateTime> beginDateRef = BeginDatePicker().Date();
	IReference<DateTime> endDateRef = EndDatePicker().Date();
	if (beginDateRef != nullptr && endDateRef != nullptr) {
		// DateTimePick 返回的时钟震荡周期很奇怪，与std::chrono::system_clock::now()的周期大约相差 11644473596UL，
		// 目前原因尚不清楚。
		// 通过乘以 std::chrono::system_clock::period::num / std::chrono::system_clock::period::den  - 11644473596UL
		// 转化为实际的秒数
		time_t bt = beginDateRef.Value().time_since_epoch().count() * std::chrono::system_clock::period::num / std::chrono::system_clock::period::den;
		bt = static_cast<time_t>(static_cast<uint64_t>(bt) - 11644473596UL);
		time_t et = endDateRef.Value().time_since_epoch().count() * std::chrono::system_clock::period::num / std::chrono::system_clock::period::den;
		et = static_cast<time_t>(static_cast<uint64_t>(et) - 11644473596UL);

		const Janyee::DateTime& beginDateTime = Janyee::DateTime(bt);
		const Janyee::DateTime& endDateTime = Janyee::DateTime(et);
		OutputDebugStringA(("beginDateTime: " + beginDateTime.ToString() + "\n").c_str());
		OutputDebugStringA(("endDateTime: " + endDateTime.ToString() + "\n").c_str());

		std::shared_ptr<std::wstringstream> s = std::make_shared<std::wstringstream>();
		for (uint32_t i = 0; i < EntriesTotal().Value(); i++) {
			try {
				const auto& entry = MainPageHelper::RandomDateTime(beginDateTime, endDateTime);
				*s << Janyee::Utilty::StringToWstring(entry.ToShortDate())
					<< L" x"
					<< std::to_wstring(
						static_cast<int>(MainPageHelper::RandomEventFrequency(EventFrequencyDownLimit().Value(), EventFrequencyUpperLimit().Value()))
					)
					<< L"\n";
				OutputDebugStringA(Janyee::Utilty::WstringToString(s->str()).c_str());

				auto picker = FolderPicker::FolderPicker();
				picker.ViewMode(PickerViewMode::Thumbnail);
				picker.SuggestedStartLocation(PickerLocationId::Downloads);
				picker.FileTypeFilter().Append(L".txt");
				auto folder = co_await picker.PickSingleFolderAsync();
				if (folder != nullptr) {
					Windows::Storage::AccessCache::StorageApplicationPermissions::FutureAccessList().AddOrReplace(L"PickedFolderToken", folder);
					co_await folder.CreateFileAsync(L"sample.txt", Windows::Storage::CreationCollisionOption::ReplaceExisting);
					auto file = co_await folder.GetFileAsync(L"sample.txt");
					co_await Windows::Storage::FileIO::WriteTextAsync(file, s->str());
					GeneratorTip().IsOpen(true);
				}
				s->clear();
			}
			catch (const Janyee::DateTimeException & e) {
				OutputDebugStringA(e.what());
				throw e;
			}
		}
	}
	else {
		std::shared_ptr<ContentDialog> c = std::make_shared<ContentDialog>();
		c->Title(box_value(L"Error"));
		c->Content(box_value(L"You not pick a date."));
		c->CloseButtonText(L"Close");
		co_await c->ShowAsync();
		//std::string* s = new std::string("aaaaaaaaaaaaaaaaaaaaaaaaaaa");
	}
}
