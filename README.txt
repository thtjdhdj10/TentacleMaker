- Tentacle Maker 구조 -


사용자가 준비한 GameObject 에 <TentacleRoot> 스크립트를 추가하여 생성할 수 있다.

이 스크립트를 통해 그 root 에 달린 TentacleBody 를 제어할 수 있다.

AddBodyObjects(List<GameObject> obj) 함수로 몸체를 연장시킬 수 있다.

인자로 들어간 오브젝트들에는 <TentacleBody> 스크립트가 추가되며, 이에 따라 transform 이 변한다.


TentacleRoot에는 촉수의 속성값인 TentacleProperties 을 public static Dictionary 로 가지고 있다.

public 이기 때문에 새로운 촉수의 속성을 정의하고 싶다면 직접 접근해서 수정하면 된다.

key 값은 string 이므로, 속성의 이름을 적당히 설정해서 쓰면 됨.


각 root 는 속성값의 이름 중 하나를 public 멤버로 가지고 있다. 이 값을 통해 root 의 속성을 바꿀 수 있다.



현재 구현된 촉수의 속성들은 아래와 같다.

	// 휜 정도의 회복 속도
        // recovery speed of bending
        public float bendingRecoverySpeed = 0.2f; // 0 ~ 1

        // 최대로 휘는 정도
        // max bending degrees of the two objects
        public float bendingDegrees = 7.5f;

        // 변형 회복 속도
        // form recovery speed by movement
        public float modifyRecoverySpeed = 0.2f; // 0 ~ 1

        // 변형에 의한 회전속도
        // rotation speed by modify
        public float rotationSpeedByModify = 0.5f; // 0 ~ 1

        // 최대 길이는 deltaTheta, deltaPhi 에 따라
        // baseLength + baseLength * lengthenDegrees 가 된다.

        // 길어지는 비율
        public float lengthenRatio = 2.0f;

        // object 사이의 최소 간격( lengthenRatio = 0 일때 길이는 이 값으로 고정됨 )
        public float minLength = 0.5f;

        public bool solid = false;

        // 일단 임시로 모든 실린더 on off 여기서 관리.
        // TODO body 에서 관리하게 수정
        public bool useCylinder = true;

        public string cylinder = "Cylinder";

        // TODO 얇아지는 비율

        // 굵기
        public float thickness = 1.0f;

        public float headSize = 1.0f;
        public float tailSize = 1.0f;
