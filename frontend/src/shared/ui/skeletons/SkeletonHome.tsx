export default function SkeletonHome() {
  return (
    <div className="w-full h-full flex flex-col items-center justify-center gap-6">
      <div className="w-full space-y-8 animate-pulse">
        <div className="w-full bg-base-200 rounded-2xl skeleton p-6">
          <div className="flex items-center justify-between mb-4">
            <div className="flex items-center gap-3">
              <div className="bg-base-300 p-3 rounded-2xl w-14 h-14"></div>
              <div className="space-y-2">
                <div className="bg-base-300 rounded h-4 w-16"></div>
                <div className="bg-base-300 rounded h-8 w-12"></div>
              </div>
            </div>
            <div className="bg-base-300 backdrop-blur-sm px-4 py-2 rounded-full w-24 h-9"></div>
          </div>

          <div className="space-y-2">
            <div className="flex justify-between text-sm">
              <div className="bg-base-300 rounded h-4 w-40"></div>
              <div className="bg-base-300 rounded h-4 w-20"></div>
            </div>
            <div className="bg-base-300 rounded-full h-3 w-full"></div>
          </div>
        </div>

        <div className="grid grid-cols-2 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          {Array.from({ length: 4 }).map((_, index) => (
            <div
              key={index}
              className="bg-base-200 p-6 rounded-2xl shadow-xl relative overflow-hidden skeleton"
            >
              <div className="absolute top-0 right-0 w-24 h-24 bg-base-300 rounded-full -mr-12 -mt-12" />
              <div className="relative z-10">
                <div className="w-8 h-8 bg-base-300 rounded-lg mb-3"></div>
                <div className="bg-base-300 rounded h-4 w-20 mb-2"></div>
                <div className="bg-base-300 rounded h-8 w-16"></div>
              </div>
            </div>
          ))}
        </div>

        <div role="tablist" className="tabs tabs-border gap-2">
          <div className="tab gap-2 bg-base-200 skeleton">
            <div className="w-5 h-5 bg-base-300 rounded"></div>
            <div className="bg-base-300 rounded h-4 w-12"></div>
          </div>
          <div className="tab gap-2 bg-base-200 skeleton">
            <div className="w-5 h-5 bg-base-300 rounded"></div>
            <div className="bg-base-300 rounded h-4 w-16"></div>
          </div>
        </div>

        <div className="space-y-6 mb-12">
          <div className="space-y-6">
            <div className="flex items-center justify-between ">
              <div className="bg-base-200 rounded h-8 w-40 skeleton"></div>
              <div className="flex items-center gap-2 opacity-70">
                <div className="bg-base-300 rounded h-4 w-16"></div>
                <div className="w-8 h-8 bg-base-300 rounded-full"></div>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
              {[...Array(4)].map((_, index) => (
                <div
                  key={index}
                  className="bg-base-200 rounded-xl p-6 skeleton"
                >
                  <div className="flex items-start justify-between mb-4">
                    <div className="relative">
                      <div className="bg-base-300 p-3 rounded-2xl w-14 h-14"></div>
                    </div>
                    <div className="bg-base-300 px-3 py-1 rounded-full w-16 h-6"></div>
                  </div>

                  <div className="flex items-center justify-between mb-4">
                    <div className="bg-base-300 rounded h-6 w-24"></div>
                    <div className="bg-base-300 rounded-xl w-20 h-8"></div>
                  </div>

                  <div className="space-y-2">
                    <div className="bg-base-300 rounded h-4 w-28"></div>
                    <div className="bg-base-300 rounded-full h-2 w-full"></div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
