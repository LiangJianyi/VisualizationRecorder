#include "pch.h"
#include "MainPage.h"
#include "MainPage.g.cpp"
#include "MainPageHelper.h"
#include <string>


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
	GeneratorButton().Visibility(Visibility::Collapsed);
	Ring().IsActive(true);
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
		auto picker = FolderPicker::FolderPicker();
		picker.ViewMode(PickerViewMode::Thumbnail);
		picker.SuggestedStartLocation(PickerLocationId::Downloads);
		picker.FileTypeFilter().Append(L".txt");
		auto folder = co_await picker.PickSingleFolderAsync();
		co_await GenerateDateEntryAsync(folder, s, beginDateTime, endDateTime);
		Ring().IsActive(false);
		GeneratorTip().IsOpen(true);
		GeneratorButton().Visibility(Visibility::Collapsed);
	}
	else {
		std::shared_ptr<ContentDialog> c = std::make_shared<ContentDialog>();
		c->Title(box_value(L"Error"));
		c->Content(box_value(L"You not pick a date."));
		c->CloseButtonText(L"Close");
		co_await c->ShowAsync();
	}
}

IAsyncAction winrt::TxtRecordGenerator::implementation::MainPage::GenerateDateEntryAsync(StorageFolder folder, std::shared_ptr<std::wstringstream> s, Janyee::DateTime beginDateTime, Janyee::DateTime endDateTime) {
	auto strong_this { get_strong() };
	if (folder != nullptr) {
		Windows::Storage::AccessCache::StorageApplicationPermissions::FutureAccessList().AddOrReplace(L"PickedFolderToken", folder);
		co_await folder.CreateFileAsync(L"sample.txt", Windows::Storage::CreationCollisionOption::ReplaceExisting);
		auto file = co_await folder.GetFileAsync(L"sample.txt");
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

			}
			catch (const Janyee::DateTimeException & e) {
				OutputDebugStringA(e.what());
				throw e;
			}
			catch (const std::exception & e) {
				OutputDebugStringA(e.what());
				throw e;
			}
			co_await Windows::Storage::FileIO::WriteTextAsync(file, s->str());
			s->clear();
		}
	}
}
